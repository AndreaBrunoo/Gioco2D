# COMMESSA DI SVILUPPO – GIOCO2D

## 1. Panoramica del Progetto

Gioco2D è un RPG Single Player browser-based sviluppato tramite architettura client-server.

Il gioco combina esplorazione libera di una mappa 2D a celle con combattimenti tattici a turni, progressione del personaggio, gestione dell'equipaggiamento, NPC interattivi, quest e sistema di persistenza dei dati.

L'obiettivo è realizzare una prima versione completa (MVP) che consenta al giocatore di creare un personaggio, esplorare il mondo di gioco, affrontare nemici e boss, completare missioni e progredire attraverso diverse zone.

---

# 2. Tecnologie

## Backend

* ASP.NET Core Web API (.NET 9)
* Entity Framework Core
* PostgreSQL
* JWT Authentication
* Clean Architecture
* Dependency Injection
* Repository Pattern
* CQRS leggero

## Frontend

* Angular 21
* RxJS
* Standalone Components
* Lazy Loading
* HTML5 Canvas per rendering mappa e combattimento
* TypeScript

---

# 3. Modalità di Gioco

Il gioco è esclusivamente Single Player.

Tutta la logica critica viene gestita dal backend:

* Progressione personaggio
* Calcolo esperienza
* Livelli
* Inventario
* Equipaggiamento
* Combattimento
* Loot
* Quest
* Salvataggi

Il frontend si occupa esclusivamente di:

* Rendering
* Input utente
* Interfaccia grafica
* Comunicazione con API

---

# 4. Gameplay Loop

```text
Esplorazione
↓
Incontro Nemici
↓
Combattimento Tattico
↓
Ricompense
↓
Livellamento
↓
Miglioramento Equipaggiamento
↓
Esplorazione
```

---

# 5. Creazione Personaggio

All'inizio della partita il giocatore deve scegliere una delle classi disponibili.

## Guerriero

Caratteristiche:

* HP elevati
* Difesa elevata
* Specializzato nel combattimento corpo a corpo

## Arciere

Caratteristiche:

* Attacchi a distanza
* Elevata mobilità
* Maggiore probabilità di colpo critico

## Mago

Caratteristiche:

* Danni magici elevati
* Attacchi ad area
* Utilizzo di mana

---

# 6. Sistema Statistiche

Ogni personaggio possiede le seguenti statistiche:

| Statistica   | Effetto                |
| ------------ | ---------------------- |
| Vitalità     | HP massimi             |
| Forza        | Danno fisico           |
| Destrezza    | Precisione e schivata  |
| Intelligenza | Danno magico           |
| Difesa       | Riduzione danni        |
| Velocità     | Iniziativa e movimento |

---

# 7. Sistema Livelli

## Livello massimo

50

## Esperienza

L'esperienza viene ottenuta tramite:

* Uccisione nemici
* Uccisione boss
* Completamento quest

## Avanzamento

Ad ogni livello il personaggio ottiene:

* 5 punti statistica distribuibili liberamente

---

# 8. Mondo di Gioco

Il mondo è suddiviso in zone collegate tra loro.

Ogni zona rappresenta una fase della progressione del giocatore.

Le zone possono essere attraversate liberamente, ma la difficoltà aumenta progressivamente.

---

# 9. Tipologie di Zone

## Villaggi

Zone sicure.

Caratteristiche:

* Nessun nemico
* Locanda
* Mercanti
* NPC
* Quest giver

## Zone di Esplorazione

Zone contenenti:

* Nemici
* NPC opzionali
* Boss di zona

---

# 10. Mappa

La mappa utilizza una griglia a celle.

Tipologie di celle:

## Percorribili

* Erba
* Strade
* Pavimentazioni

## Non Percorribili

* Acqua
* Alberi
* Rocce
* Ostacoli

---

# 11. Sistema Nemici

Ogni zona possiede un proprio livello consigliato.

Esempio:

| Zona   | Livello |
| ------ | ------- |
| Zona 1 | 1-5     |
| Zona 2 | 6-10    |
| Zona 3 | 11-15   |
| Zona 4 | 16-20   |
| Zona 5 | 21-25   |

I nemici della zona vengono generati utilizzando archetipi configurabili e scalano automaticamente in base al livello della zona.

Ogni nemico possiede:

* Nome
* Livello
* HP
* Statistiche
* Esperienza fornita
* Tabella drop
* Eventuali abilità speciali

I nemici possono respawnare dopo un intervallo configurabile.

---

# 12. Boss di Zona

Ogni zona non sicura contiene un Boss.

Il Boss rappresenta la sfida principale della zona.

## Livello Boss

Il Boss utilizza il livello massimo previsto per la zona.

Esempio:

| Zona  | Boss            |
| ----- | --------------- |
| 1-5   | Boss livello 5  |
| 6-10  | Boss livello 10 |
| 11-15 | Boss livello 15 |

## Caratteristiche

* HP elevati
* Statistiche avanzate
* Abilità speciali
* Loot migliorato

## Ricompense

* Esperienza bonus
* Oro
* Oggetti rari garantiti
* Sblocco della progressione successiva

I Boss sconfitti vengono salvati permanentemente.

Per l'MVP non respawnano.

---

# 13. Sistema Combattimento

## Attivazione

Quando il giocatore entra nel raggio di ingaggio di un nemico viene attivata la modalità combattimento.

La mappa passa alla modalità tattica a turni.

---

## Griglia di Combattimento

Dimensione configurabile.

Riferimento iniziale:

12x12 celle.

---

## Turni

L'ordine di turno viene determinato dalla statistica Velocità.

---

## Azioni Disponibili

Durante il proprio turno un'unità può:

* Muoversi
* Attaccare
* Utilizzare un oggetto
* Terminare il turno

---

## Movimento

Numero celle percorribili:

```text
3 + (Velocità / 10)
```

---

## Attacchi Corpo a Corpo

Richiedono adiacenza al bersaglio.

Distanza:

1 cella.

---

## Attacchi a Distanza

Dipendono dall'arma equipaggiata.

Richiedono bersaglio entro il raggio previsto.

---

## Magie

Tipologie supportate:

* Bersaglio singolo
* Area di effetto

Le magie consumano mana.

---

# 14. Sistema Danni

## Danno Fisico

```text
Danno = Attacco - Difesa
```

Danno minimo:

```text
1
```

## Danno Magico

```text
Danno = Potere Magico - Resistenza Magica
```

Danno minimo:

```text
1
```

---

# 15. Effetti Speciali

Sistema estendibile.

Effetti iniziali:

* Colpo critico
* Veleno
* Bruciatura
* Stordimento
* Rallentamento

---

# 16. IA Nemici

Comportamento base:

1. Individua il bersaglio più vicino.
2. Se può attaccare, attacca.
3. Altrimenti si avvicina.
4. Utilizza abilità se disponibili.

---

# 17. Equipaggiamento

Slot disponibili:

* Arma
* Elmo
* Torace
* Guanti
* Stivali
* Accessorio

## Rarità

* Comune
* Non Comune
* Raro
* Epico
* Leggendario

---

# 18. Inventario

Capienza iniziale:

40 slot.

Supporta:

* Stack consumabili
* Stack materiali
* Equipaggiamenti singoli

---

# 19. Sistema Loot

I nemici possono rilasciare:

* Oro
* Armi
* Armature
* Pozioni
* Materiali

Le probabilità di drop sono configurabili da database.

---

# 20. NPC

Tipologie supportate.

## NPC Dialogo

Solo conversazioni.

## Quest Giver

Assegnano missioni.

## Mercanti

Consentono acquisto e vendita di oggetti.

---

# 21. Sistema Quest

Tipologie MVP.

## Eliminazione

Esempio:

Uccidi 10 Lupi.

## Raccolta

Esempio:

Raccogli 5 Pelli.

## Esplorazione

Esempio:

Raggiungi una determinata zona.

---

## Ricompense

* Esperienza
* Oro
* Oggetti

---

# 22. Sistema Mercanti

Funzionalità:

* Acquisto oggetti
* Vendita oggetti

I prezzi vengono definiti da database.

---

# 23. Locande e Checkpoint

Ogni villaggio possiede una locanda.

Interagendo con una locanda:

* viene aggiornato il checkpoint del personaggio.

Alla morte:

* respawn presso l'ultima locanda visitata.

Penalità:

* perdita del 10% dell'oro posseduto.

L'equipaggiamento viene mantenuto.

---

# 24. Sistema Salvataggio

Salvataggio automatico fuori dal combattimento.

Eventi che attivano il salvataggio:

* Cambio zona
* Completamento quest
* Visita locanda
* Intervallo periodico

Dati salvati:

* Posizione
* Livello
* Esperienza
* Statistiche
* Inventario
* Equipaggiamento
* Quest
* Boss sconfitti
* Checkpoint

---

# 25. API Backend

Moduli principali:

```text
/api/auth
/api/characters
/api/maps
/api/combat
/api/inventory
/api/equipment
/api/npcs
/api/merchants
/api/quests
/api/saves
```

---

# 26. MVP Iniziale

Contenuti minimi richiesti.

* 3 classi giocabili
* 1 villaggio
* 5 zone esplorabili
* 5 boss
* Circa 25-30 tipologie di nemici
* 10-15 quest
* Sistema combattimento completo
* Sistema inventario
* Sistema equipaggiamento
* Sistema mercanti
* Sistema checkpoint
* Sistema salvataggio automatico
* Persistenza completa tramite database PostgreSQL

Il progetto dovrà essere sviluppato in modo modulare e facilmente estendibile per consentire l'aggiunta futura di nuove classi, zone, boss, quest, abilità e sistemi di gioco senza modifiche sostanziali all'architettura esistente.
