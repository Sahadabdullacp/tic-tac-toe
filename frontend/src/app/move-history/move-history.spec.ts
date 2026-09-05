import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { GameState } from '../game.model';
import { GameService } from '../game.service';
import { MoveHistoryComponent } from './move-history';

const aGame = (overrides: Partial<GameState> = {}): GameState => ({
  id: 'game-1',
  board: Array(9).fill(null),
  currentPlayer: 'X',
  mode: 'TwoPlayer',
  status: 'InProgress',
  winner: null,
  winningCells: [],
  history: [],
  scoreboard: { xWins: 0, oWins: 0, draws: 0 },
  ...overrides,
});

describe('MoveHistoryComponent', () => {
  let gameService: GameService;

  const createComponent = () => {
    const fixture = TestBed.createComponent(MoveHistoryComponent);
    fixture.detectChanges();
    return fixture;
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [MoveHistoryComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    gameService = TestBed.inject(GameService);
  });

  it('shows a placeholder when no moves have been made', () => {
    gameService.game.set(aGame());
    const fixture = createComponent();

    expect(fixture.nativeElement.querySelector('.empty').textContent).toContain('No moves yet.');
  });

  it('lists each move with its player and board position', () => {
    gameService.game.set(
      aGame({
        history: [
          { number: 1, player: 'X', cell: 0 },
          { number: 2, player: 'O', cell: 4 },
        ],
      }),
    );
    const fixture = createComponent();

    const rows = fixture.nativeElement.querySelectorAll('.history-row:not(.header)');
    expect(rows.length).toBe(2);
    expect(rows[0].textContent).toContain('Row 1, Column 1');
    expect(rows[1].textContent).toContain('Row 2, Column 2');
  });
});
