import { Component, computed, inject, OnInit } from '@angular/core';
import { GameBoardComponent } from './game-board/game-board';
import { GameMode } from './game.model';
import { GameService } from './game.service';
import { MoveHistoryComponent } from './move-history/move-history';
import { ScoreboardPanelComponent } from './scoreboard-panel/scoreboard-panel';

@Component({
  imports: [GameBoardComponent, MoveHistoryComponent, ScoreboardPanelComponent],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App implements OnInit {
  private readonly gameService = inject(GameService);

  readonly game = this.gameService.game;
  readonly errorMessage = this.gameService.errorMessage;

  readonly canUndo = computed(() => (this.game()?.history.length ?? 0) > 0 && this.game()?.status === 'InProgress');

  readonly statusMessage = computed(() => {
    const game = this.game();
    if (!game) return '';
    if (game.status === 'Won') return `Player ${game.winner} wins!`;
    if (game.status === 'Draw') return "It's a draw!";
    return `Player ${game.currentPlayer}'s turn`;
  });

  ngOnInit(): void {
    void this.gameService.startGame('TwoPlayer');
  }

  onSelectMode(mode: GameMode): void {
    void this.gameService.startGame(mode);
  }

  onUndo(): void {
    void this.gameService.undo();
  }

  onReset(): void {
    void this.gameService.reset();
  }
}
