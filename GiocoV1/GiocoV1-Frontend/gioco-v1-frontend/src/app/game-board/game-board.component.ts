import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GameService, GameState } from '../services/game.service';

@Component({
  selector: 'app-game-board',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './game-board.component.html',
  styleUrls: ['./game-board.component.css']
})
export class GameBoardComponent implements OnInit {
  gameState: GameState | null = null;
  loading = false;
  error: string | null = null;

  constructor(private gameService: GameService) { }

  ngOnInit(): void {
    this.loadGameState();
  }

  loadGameState(): void {
    this.loading = true;
    this.error = null;
    this.gameService.getState().subscribe({
      next: (state) => {
        this.gameState = state;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching game state', err);
        this.error = 'Impossibile caricare lo stato del gioco';
        this.loading = false;
      }
    });
  }

  performAction(action: string): void {
    this.loading = true;
    this.error = null;
    this.gameService.performAction(action).subscribe({
      next: (state) => {
        this.gameState = state;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error performing action', err);
        this.error = 'Errore durante l\'esecuzione dell\'azione';
        this.loading = false;
      }
    });
  }
}
