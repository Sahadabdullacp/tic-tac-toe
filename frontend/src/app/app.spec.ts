import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../environments/environment';
import { App } from './app';
import { GameState } from './game.model';

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

describe('App', () => {
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('starts a two-player game on init', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/games`);
    expect(req.request.body).toEqual({ mode: 'TwoPlayer' });
    req.flush(aGame());
    await fixture.whenStable();

    expect(fixture.componentInstance.game()?.id).toBe('game-1');
  });

  it('shows whose turn it is while the game is in progress', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    httpMock.expectOne(`${environment.apiBaseUrl}/games`).flush(aGame({ currentPlayer: 'O' }));
    await fixture.whenStable();
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('.status-banner').textContent).toContain("Player O's turn");
  });

  it('announces the winner once the game is won', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    httpMock.expectOne(`${environment.apiBaseUrl}/games`).flush(aGame({ status: 'Won', winner: 'X' }));
    await fixture.whenStable();
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('.status-banner').textContent).toContain('Player X wins!');
  });

  it('announces a draw', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    httpMock.expectOne(`${environment.apiBaseUrl}/games`).flush(aGame({ status: 'Draw' }));
    await fixture.whenStable();
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('.status-banner').textContent).toContain("It's a draw!");
  });

  it('disables undo when there is no move history', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    httpMock.expectOne(`${environment.apiBaseUrl}/games`).flush(aGame());
    await fixture.whenStable();

    expect(fixture.componentInstance.canUndo()).toBe(false);
  });

  it('enables undo once moves exist and the game is still in progress', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    httpMock
      .expectOne(`${environment.apiBaseUrl}/games`)
      .flush(aGame({ history: [{ number: 1, player: 'X', cell: 0 }] }));
    await fixture.whenStable();

    expect(fixture.componentInstance.canUndo()).toBe(true);
  });

  it('disables undo once the game is over even with move history', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    httpMock.expectOne(`${environment.apiBaseUrl}/games`).flush(
      aGame({ status: 'Won', winner: 'X', history: [{ number: 1, player: 'X', cell: 0 }] }),
    );
    await fixture.whenStable();

    expect(fixture.componentInstance.canUndo()).toBe(false);
  });

  it('starts a new game when a different mode is selected', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    httpMock.expectOne(`${environment.apiBaseUrl}/games`).flush(aGame());
    await fixture.whenStable();
    fixture.detectChanges();

    fixture.nativeElement.querySelectorAll('.mode-toggle button')[1].click();

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/games`);
    expect(req.request.body).toEqual({ mode: 'VsComputer' });
    req.flush(aGame({ mode: 'VsComputer' }));
    await fixture.whenStable();
  });
});
