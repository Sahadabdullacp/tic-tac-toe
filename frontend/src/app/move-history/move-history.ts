import { Component, computed, inject } from '@angular/core';
import { GameService } from '../game.service';

@Component({
  selector: 'app-move-history',
  standalone: true,
  templateUrl: './move-history.html',
  styleUrl: './move-history.scss',
})
export class MoveHistoryComponent {
  private readonly gameService = inject(GameService);

  readonly moves = computed(() => this.gameService.game()?.history ?? []);

  positionLabel(cell: number): string {
    const row = Math.floor(cell / 3) + 1;
    const column = (cell % 3) + 1;
    return `Row ${row}, Column ${column}`;
  }
}
