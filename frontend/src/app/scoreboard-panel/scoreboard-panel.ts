import { Component, inject } from '@angular/core';
import { GameService } from '../game.service';

@Component({
  selector: 'app-scoreboard-panel',
  standalone: true,
  templateUrl: './scoreboard-panel.html',
  styleUrl: './scoreboard-panel.scss',
})
export class ScoreboardPanelComponent {
  protected readonly gameService = inject(GameService);

  onResetScoreboard(): void {
    void this.gameService.resetScoreboard();
  }
}
