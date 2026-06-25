import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface GameState {
  versioneGioco: string;
  timestamp: string;
  personaggio: {
    nome: string;
    saluteMassima: number;
    saluteAttuale: number;
    attacco: number;
    difesa: number;
    velocita: number;
    livello: number;
    esperienza: number;
    mosse: any[]; // We'll define a proper type later if needed
    inventario: any[];
    equipaggiamenti: any;
    monete: number;
    puntiAbilita: number;
    posX: number;
    posY: number;
  };
  areaCorrente: string;
  posizioneX: number;
  posizioneY: number;
  bossSconfitti: string[];
  oggettiRaccolti: string[];
  nemiciEliminati: string[];
  eventiCompletati: string[];
}

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private apiUrl = `${environment.apiUrl}/game`;

  constructor(private http: HttpClient) { }

  getState(): Observable<GameState> {
    return this.http.get<GameState>(`${this.apiUrl}/state`);
  }

  performAction(action: string): Observable<GameState> {
    return this.http.post<GameState>(`${this.apiUrl}/action`, action, { responseType: 'json' });
  }
}
