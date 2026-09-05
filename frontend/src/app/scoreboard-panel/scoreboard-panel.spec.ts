import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { vi } from 'vitest';
import { GameState } from '../game.model';
import { GameService } from '../game.service';
import { ScoreboardPanelComponent } from './scoreboard-panel';

const aGame = (overrides: Partial<GameState> = {}): GameState => ({
  id: 'game-1',
  board: Array(9).fill(null),
  currentPlayer: 'X',
  mode: 'TwoPlayer',
  status: 'InProgress',
  winner: null,
  winningCells: [],
  history: [],
  scoreboard: { xWins: 2, oWins: 1, draws: 3 },
  ...overrides,
});

describe('ScoreboardPanelComponent', () => {
  let gameService: GameService;

  const createComponent = () => {
    const fixture = TestBed.createComponent(ScoreboardPanelComponent);
    fixture.detectChanges();
    return fixture;
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ScoreboardPanelComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    gameService = TestBed.inject(GameService);
  });

  it('displays the current win and draw tallies', () => {
    gameService.game.set(aGame());
    const fixture = createComponent();

    const text = fixture.nativeElement.textContent;
    expect(text).toContain('2');
    expect(text).toContain('1');
    expect(text).toContain('3');
  });

  it('resets the scoreboard when the button is clicked', () => {
    gameService.game.set(aGame());
    const fixture = createComponent();
    vi.spyOn(gameService, 'resetScoreboard');

    fixture.nativeElement.querySelector('.link-button').click();

    expect(gameService.resetScoreboard).toHaveBeenCalled();
  });
});
