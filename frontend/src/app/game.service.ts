import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { catchError, firstValueFrom, of, type Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { ApiError, GameMode, GameState, Scoreboard } from './game.model';

const COMPUTER_THINKING_DELAY_MS = 500;

/** Talks to the backend REST API and holds the current game as the single source of truth. */
@Injectable({ providedIn: 'root' })
export class GameService {
  private readonly http = inject(HttpClient);
  private readonly apiBase = environment.apiBaseUrl;

  readonly game = signal<GameState | null>(null);
  readonly errorMessage = signal<string | null>(null);
  readonly computerThinking = signal(false);

  async startGame(mode: GameMode): Promise<void> {
    const state = await firstValueFrom(
      this.http.post<GameState>(`${this.apiBase}/games`, { mode }),
    );
    this.errorMessage.set(null);
    this.game.set(state);
  }

  async move(cell: number): Promise<void> {
    const current = this.game();
    if (!current) return;

    if (this.triggersComputerTurn(current)) await this.playHumanMoveThenLetComputerThink(current, cell);

    await this.applyAction(
      this.http.post<GameState>(`${this.apiBase}/games/${current.id}/moves`, {
        player: current.currentPlayer,
        cell,
      }),
    );
    this.computerThinking.set(false);
  }

  private triggersComputerTurn(game: GameState): boolean {
    return game.mode === 'VsComputer' && game.currentPlayer === 'X';
  }

  /** Renders the human's mark immediately, then pauses so the computer's reply doesn't feel instant. */
  private async playHumanMoveThenLetComputerThink(current: GameState, cell: number): Promise<void> {
    this.game.set(this.withOptimisticMove(current, cell));
    this.computerThinking.set(true);
    await this.wait(COMPUTER_THINKING_DELAY_MS);
  }

  private withOptimisticMove(game: GameState, cell: number): GameState {
    const board = [...game.board];
    board[cell] = game.currentPlayer;
    return { ...game, board };
  }

  private wait(ms: number): Promise<void> {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }

  async undo(): Promise<void> {
    const current = this.game();
    if (!current) return;

    await this.applyAction(
      this.http.post<GameState>(`${this.apiBase}/games/${current.id}/undo`, {}),
    );
  }

  async reset(): Promise<void> {
    const current = this.game();
    if (!current) return;

    await this.applyAction(
      this.http.post<GameState>(`${this.apiBase}/games/${current.id}/reset`, {}),
    );
  }

  async resetScoreboard(): Promise<void> {
    const scoreboard = await firstValueFrom(
      this.http.post<Scoreboard>(`${this.apiBase}/scoreboard/reset`, {}),
    );
    const current = this.game();
    if (current) this.game.set({ ...current, scoreboard });
  }

  /** Runs a state-changing request, updating the game on success or surfacing the backend's error message. */
  private async applyAction(request: Observable<GameState>): Promise<void> {
    const result = await firstValueFrom(
      request.pipe(catchError((response: HttpErrorResponse) => of(this.toApiError(response)))),
    );

    if (this.isApiError(result)) {
      this.errorMessage.set(result.error);
    } else {
      this.errorMessage.set(null);
      this.game.set(result);
    }
  }

  private toApiError(response: HttpErrorResponse): ApiError {
    return response.error ?? { error: 'Something went wrong. Please try again.' };
  }

  private isApiError(result: GameState | ApiError): result is ApiError {
    return 'error' in result;
  }
}
