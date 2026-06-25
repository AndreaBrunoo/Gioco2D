import { Routes } from '@angular/router';
import { GameBoardComponent } from './game-board/game-board.component';

export const routes: Routes = [
  { path: '', redirectTo: 'game', pathMatch: 'full' },
  { path: 'game', component: GameBoardComponent }
];
