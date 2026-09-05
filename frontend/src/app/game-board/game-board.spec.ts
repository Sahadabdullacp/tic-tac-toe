import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { vi } from 'vitest';
import { GameState } from '../game.model';
import { GameService } from '../game.service';
import { GameBoardComponent } from './game-board';

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

describe('GameBoardComponent', () => {
  let gameService: GameService;

  const createComponent = () => {
    const fixture = TestBed.createComponent(GameBoardComponent);
    fixture.detectChanges();
    return fixture;
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GameBoardComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    gameService = TestBed.inject(GameService);
  });

  it('renders nine empty cells for a fresh game', () => {
    gameService.game.set(aGame());
    const fixture = createComponent();

    const cells = fixture.nativeElement.querySelectorAll('.cell');
    expect(cells.length).toBe(9);
    expect([...cells].every((cell: HTMLElement) => cell.textContent?.trim() === '')).toBe(true);
  });

  it('renders X and O marks from the board', () => {
    gameService.game.set(aGame({ board: ['X', 'O', null, null, null, null, null, null, null] }));
    const fixture = createComponent();

    const cells = fixture.nativeElement.querySelectorAll('.cell');
    expect(cells[0].textContent.trim()).toBe('X');
    expect(cells[1].textContent.trim()).toBe('O');
  });

  it('highlights the winning cells', () => {
    gameService.game.set(
      aGame({ status: 'Won', winner: 'X', board: ['X', 'X', 'X', null, null, null, null, null, null], winningCells: [0, 1, 2] }),
    );
    const fixture = createComponent();

    const cells = fixture.nativeElement.querySelectorAll('.cell');
    expect(cells[0].classList.contains('winning')).toBe(true);
    expect(cells[3].classList.contains('winning')).toBe(false);
  });

  it('disables occupied cells and cells once the game is over', () => {
    gameService.game.set(aGame({ status: 'Draw', board: ['X', null, null, null, null, null, null, null, null] }));
    const fixture = createComponent();

    const cells = fixture.nativeElement.querySelectorAll('.cell');
    expect(cells[0].disabled).toBe(true);
    expect(cells[1].disabled).toBe(true);
  });

  it('disables the board while it is the computer\'s turn', () => {
    gameService.game.set(aGame({ mode: 'VsComputer', currentPlayer: 'O' }));
    const fixture = createComponent();

    const cells = fixture.nativeElement.querySelectorAll('.cell');
    expect([...cells].every((cell: HTMLElement) => (cell as HTMLButtonElement).disabled)).toBe(true);
  });

  it('submits a move when an empty playable cell is clicked', () => {
    gameService.game.set(aGame());
    const fixture = createComponent();
    vi.spyOn(gameService, 'move');

    fixture.nativeElement.querySelectorAll('.cell')[4].click();

    expect(gameService.move).toHaveBeenCalledWith(4);
  });
});
