import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../environments/environment';
import { GameState } from './game.model';
import { GameService } from './game.service';

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

describe('GameService', () => {
  let service: GameService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    service = TestBed.inject(GameService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('starts with no game and no error', () => {
    expect(service.game()).toBeNull();
    expect(service.errorMessage()).toBeNull();
  });

  it('stores the game returned when starting a game', async () => {
    const startPromise = service.startGame('TwoPlayer');

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/games`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ mode: 'TwoPlayer' });
    req.flush(aGame());
    await startPromise;

    expect(service.game()?.id).toBe('game-1');
  });

  it('sends the current player and cell when moving', async () => {
    service.game.set(aGame());

    const movePromise = service.move(4);
    const req = httpMock.expectOne(`${environment.apiBaseUrl}/games/game-1/moves`);
    expect(req.request.body).toEqual({ player: 'X', cell: 4 });
    req.flush(aGame({ board: [null, null, null, null, 'X', null, null, null, null], currentPlayer: 'O' }));
    await movePromise;

    expect(service.game()?.currentPlayer).toBe('O');
  });

  it('does nothing when moving without an active game', async () => {
    await service.move(0);
    httpMock.expectNone(`${environment.apiBaseUrl}/games/game-1/moves`);
  });

  it('surfaces the backend error message instead of updating the game on failure', async () => {
    service.game.set(aGame());

    const movePromise = service.move(0);
    const req = httpMock.expectOne(`${environment.apiBaseUrl}/games/game-1/moves`);
    req.flush({ error: 'Cell already occupied.' }, { status: 400, statusText: 'Bad Request' });
    await movePromise;

    expect(service.errorMessage()).toBe('Cell already occupied.');
    expect(service.game()?.board[0]).toBeNull();
  });

  it('clears a previous error once a later action succeeds', async () => {
    service.game.set(aGame());
    service.errorMessage.set('Cell already occupied.');

    const movePromise = service.move(4);
    httpMock.expectOne(`${environment.apiBaseUrl}/games/game-1/moves`).flush(aGame({ currentPlayer: 'O' }));
    await movePromise;

    expect(service.errorMessage()).toBeNull();
  });

  it('replaces the game with the undone state', async () => {
    service.game.set(aGame({ history: [{ number: 1, player: 'X', cell: 0 }] }));

    const undoPromise = service.undo();
    httpMock.expectOne(`${environment.apiBaseUrl}/games/game-1/undo`).flush(aGame());
    await undoPromise;

    expect(service.game()?.history).toEqual([]);
  });

  it('replaces the game with the reset state', async () => {
    service.game.set(aGame({ status: 'Draw' }));

    const resetPromise = service.reset();
    httpMock.expectOne(`${environment.apiBaseUrl}/games/game-1/reset`).flush(aGame());
    await resetPromise;

    expect(service.game()?.status).toBe('InProgress');
  });

  it('only updates the scoreboard slice when resetting the scoreboard', async () => {
    service.game.set(aGame({ scoreboard: { xWins: 3, oWins: 1, draws: 2 } }));

    const resetPromise = service.resetScoreboard();
    httpMock
      .expectOne(`${environment.apiBaseUrl}/scoreboard/reset`)
      .flush({ xWins: 0, oWins: 0, draws: 0 });
    await resetPromise;

    expect(service.game()?.scoreboard).toEqual({ xWins: 0, oWins: 0, draws: 0 });
    expect(service.game()?.id).toBe('game-1');
  });
});
