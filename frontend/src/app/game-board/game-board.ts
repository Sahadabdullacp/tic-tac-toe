import { Component, computed, inject } from '@angular/core';
import { GameMode } from '../game.model';
import { GameService } from '../game.service';

@Component({
  selector: 'app-game-board',
  standalone: true,
  templateUrl: './game-board.html',
  styleUrl: './game-board.scss',
})
export class GameBoardComponent {
  private readonly gameService = inject(GameService);

  readonly game = this.gameService.game;
  readonly cells = computed(() => this.game()?.board ?? Array(9).fill(null));
  readonly winningCells = computed(() => new Set(this.game()?.winningCells ?? []));

  isWinningCell(index: number): boolean {
    return this.winningCells().has(index);
  }

  isPlayable(index: number): boolean {
    const game = this.game();
    if (!game) return false;
    return game.status === 'InProgress' && game.board[index] === null && !this.isComputerTurn(game.mode);
  }

  private isComputerTurn(mode: GameMode): boolean {
    return mode === 'VsComputer' && this.game()?.currentPlayer === 'O';
  }

  onCellClick(index: number): void {
    if (!this.isPlayable(index)) return;
    void this.gameService.move(index);
  }
}
