# Gioco2D – Documentazione Tecnica e di Progetto Unificata

## 1. Panoramica del Progetto

Gioco2D è un RPG Single Player browser-based sviluppato tramite architettura client-server disaccoppiata.

Il gioco combina esplorazione libera di una mappa 2D a celle con combattimenti tattici a turni, progressione del personaggio, gestione dell'equipaggiamento, NPC interattivi, quest e sistema di persistenza dei dati.

L'obiettivo è realizzare una prima versione completa (MVP) che consenta al giocatore di creare un personaggio, esplorare il mondo di gioco, affrontare nemici e boss, completare missioni e progredire attraverso diverse zone.

---

## 2. Architettura del Sistema & Stack Tecnologico

### Specifica Tecnica Integrata
* **Backend:** C# .NET Web API REST.
* **Database:** SQLite per i dati persistenti.
* **Approccio:** RESTful "statico", con architettura disaccoppiata semplice.

* **Frontend:** Angular 21 (esplorazione 2D a celle, interfaccia di combattimento a griglia, RxJS, Standalone Components, Lazy Loading, TypeScript, HTML5 Canvas per rendering mappa e combattimento).
* **Autenticazione:** JWT (JSON Web Token) inserito nell'header `Authorization: Bearer <token>` per tutte le richieste protette.
* **Configurazione (Static Data):** dati di bilanciamento (mappe, statistiche nemici, tabelle di drop, NPC, quest) definiti in file JSON esterni / database, caricati in memoria (cache) all'avvio del server.

---

## 3. Modalità di Gioco e Suddivisione delle Responsabilità

Il gioco è esclusivamente **Single Player**.

Tutta la logica critica è gestita dal backend:

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
* Comunicazione con le API

### Gestione dello Stato di Combattimento
Lo stato di un combattimento attivo è memorizzato **esclusivamente in memoria (In-Memory Session)** sul server e legato all'ID utente del JWT.

* Non c'è persistenza su DB per il combattimento in corso.
* In caso di crash o riavvio del server durante un match, la sessione viene cancellata.
* Al login successivo, il frontend ripartirà dall'ultimo salvataggio automatico (avvenuto fuori dal combattimento).

---

## 4. Gameplay Loop

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

## 5. Creazione del Personaggio

All'inizio della partita il giocatore deve scegliere una delle classi disponibili.

### Guerriero
* HP elevati
* Difesa elevata
* Specializzato nel combattimento corpo a corpo

### Arciere
* Attacchi a distanza
* Elevata mobilità
* Maggiore probabilità di colpo critico

### Mago
* Danni magici elevati
* Attacchi ad area
* Utilizzo di mana

---

## 6. Sistema di Statistiche

> **Differenza tra le versioni:** la Specifica Tecnica definisce 5 statistiche core (HP, MP, Forza, Intelligenza, Destrezza), mentre la Commessa di Sviluppo definisce 6 statistiche (Vitalità, Forza, Destrezza, Intelligenza, Difesa, Velocità) senza MP esplicito. Entrambe sono riportate per riferimento.

### Statistiche Core (Specifica Tecnica)

| Statistica | Tipo | Descrizione / Impatto nel Gioco |
| :--- | :--- | :--- |
| **HP** | Risorsa | Punti Vita. Se scendono a 0, il personaggio muore e torna alla locanda. |
| **MP** | Risorsa | Punti Mana. Consumati per lanciare magie e attacchi a distanza. |
| **Forza** | Attributo | Influenza direttamente il valore di Attacco Fisico. |
| **Intelligenza** | Attributo | Determina il Danno Magico e influenza la riserva di MP. |
| **Destrezza** | Attributo | Utilizzata per il raggio di movimento sulla griglia tattica o l'iniziativa. |

## 7. Sistema di Livelli e Progressione

> **Differenza tra le versioni:** Level Cap 100 vs 50, e ricompense per livello diverse (Punti Abilità liberi vs 5 punti statistica fissi).

### Versione A
* **Level Cap:** 100.
* **Curva di Esperienza:** Esponenziale (richiede sempre più EXP per ogni livello successivo).
* **Ricompense di Livello:** Ogni level up conferisce Punti Abilità che il giocatore può spendere liberamente per incrementare le statistiche core (Forza, Destrezza, Intelligenza, HP, MP).

---

## 8. Esplorazione e Mondo di Gioco

L'esplorazione avviene in un mondo bidimensionale suddiviso in celle.

* **Movimento:** gestito da frontend tramite tasti `WASD` o `Frecce Direzionali`.
* **Struttura della Mappa:** divisa in macro-zone/zone collegate tra loro. Dimensioni, layout e numero di celle a discrezione dello sviluppatore. Ogni zona rappresenta una fase della progressione del giocatore; le zone possono essere attraversate liberamente, ma la difficoltà aumenta progressivamente.
* **Salvataggio Automatico:** il backend esegue la persistenza dello stato a ogni spostamento o azione rilevante, a patto che il giocatore si trovi **fuori dal combattimento**.
* **Morte del Personaggio:** il giocatore non perde progressi né oggetti (eccetto la penalità in oro, vedi sezione Locande), ma la sua posizione viene resettata all'ultima locanda visitata.

### Tipologie di Celle della Mappa

**Percorribili:**
* Erba
* Strade
* Pavimentazioni

**Non Percorribili:**
* Acqua
* Alberi
* Rocce
* Ostacoli

---

## 9. Tipologie di Zone

### Villaggi (Zone Neutre/Sicure)
* Nessun nemico
* Locanda (funge da checkpoint attivo)
* Mercanti (NPC dedicati alla compravendita di oggetti tramite monete)
* NPC
* Quest giver

### Zone di Esplorazione / Combattimento
Ogni zona che non sia un villaggio contiene:
* Nemici comuni
* NPC opzionali
* Un Boss di fine zona

---

## 10. Sistema Nemici

Ogni zona possiede un proprio livello consigliato. Esempio:

| Zona | Livello |
| ------ | ------- |
| Zona 1 | 1-5 |
| Zona 2 | 6-10 |
| Zona 3 | 11-15 |
| Zona 4 | 16-20 |
| Zona 5 | 21-25 |

I nemici della zona vengono generati utilizzando archetipi configurabili e scalano automaticamente in base al livello della zona. Il raggio d'azione/ingaggio è un valore variabile per ogni tipo di nemico, letto dai file di configurazione.

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

## 11. Boss di Zona

Ogni zona non sicura contiene un Boss, che rappresenta la sfida principale della zona.

### Livello Boss
Il Boss utilizza il livello massimo previsto per la zona. Esempio:

| Zona | Boss |
| ----- | --------------- |
| 1-5 | Boss livello 5 |
| 6-10 | Boss livello 10 |
| 11-15 | Boss livello 15 |

### Caratteristiche
* HP elevati
* Statistiche avanzate
* Abilità speciali
* Loot migliorato

### Ricompense
* Esperienza bonus
* Oro
* Oggetti rari garantiti
* Sblocco della progressione successiva

I Boss sconfitti vengono salvati permanentemente. Per l'MVP non respawnano.

---

## 12. Sistema di Combattimento Tattico

### Attivazione
Il combattimento si attiva in modalità asincrona non appena il personaggio, durante l'esplorazione, entra nel raggio di ingaggio di un nemico. La mappa passa alla modalità tattica a turni.

### Griglia di Combattimento

* Dimensione configurabile.
* Riferimento iniziale: 12x12 celle.
* L'azione si svolge tramite click del mouse (tasto sinistro) per muoversi e attaccare.

### Ordine dei Turni
L'ordine di turno viene determinato dalla statistica **Velocità**.

### Azioni Disponibili
Durante il proprio turno un'unità può:
* Muoversi
* Attaccare
* Utilizzare un oggetto
* Terminare il turno

### Movimento
Numero di celle percorribili:

```text
3 + (Velocità / 10)
```

### Attacchi Corpo a Corpo (Mischia)
Richiedono adiacenza al bersaglio (distanza = 1 cella).

### Attacchi a Distanza
Dipendono dall'arma equipaggiata. Richiedono bersaglio entro il raggio previsto. Possono colpire una singola cella o un'area d'effetto (AoE).

### Magie
Tipologie supportate:
* Bersaglio singolo
* Area di effetto

Le magie consumano mana.

---

## 13. Il Loop dei Turni (Single Request-Response)

Il flusso dei turni elimina la necessità di connessioni persistenti (WebSocket) sfruttando un pattern richiesta/risposta immediata:

1. Il giocatore esegue un'azione sul frontend.
2. Il frontend invia una richiesta `POST` al backend con i dettagli dell'azione.
3. Il backend elabora l'azione del giocatore, calcola i danni e applica immediatamente l'intelligenza artificiale del nemico, facendo avanzare il turno del server.
4. Il backend risponde con un unico payload contenente:
   * L'esito dell'azione del giocatore.
   * La risposta immediata (mossa/attacco) dell'IA del nemico.
   * Lo stato aggiornato della griglia e delle risorse (HP/MP di entrambi).

---

## 14. Sistema Danni

* **Danno Fisico** = Attacco - Difesa 
* **Danno Magico** = Potere Magico - Resistenza Magica

---

## 15. Effetti Speciali

Sistema estendibile. Effetti iniziali previsti:
* Colpo critico
* Veleno
* Bruciatura
* Stordimento
* Rallentamento

---

## 16. IA Nemici

Comportamento base:
1. Individua il bersaglio più vicino.
2. Se può attaccare, attacca.
3. Altrimenti si avvicina.
4. Utilizza abilità se disponibili.

---

## 17. Equipaggiamento

### Slot disponibili
* Arma
* Elmo
* Torace
* Guanti
* Stivali
* Accessorio

### Rarità
* Comune
* Non Comune
* Raro
* Epico
* Leggendario

---

## 18. Inventario

* **Capienza iniziale:** 40 slot.
* Supporta:
  * Stack consumabili
  * Stack materiali
  * Equipaggiamenti singoli

---

## 19. Sistema di Loot

I nemici e i Boss possono rilasciare:
* Oro
* Armi
* Armature
* Pozioni
* Materiali

Le probabilità di drop sono configurabili da database/JSON di configurazione (deterministico o probabilistico). Gli oggetti vengono inseriti nell'inventario persistente del giocatore.

---

## 20. NPC

### Tipologie supportate
1. **NPC Dialogo / Statici:** forniscono solo dialoghi di lore o ambientazione.
2. **Quest Giver:** assegnano missioni al giocatore.
3. **Mercanti:** presenti solo nei villaggi, consentono acquisto e vendita di oggetti.

### Interazione
Avviene tramite la pressione del tasto `E` in prossimità dell'NPC.

---

## 21. Sistema Quest

### Tipologie MVP

**Eliminazione**
Esempio: Uccidi 10 Lupi.

**Raccolta**
Esempio: Raccogli 5 Pelli.

**Esplorazione**
Esempio: Raggiungi una determinata zona.

### Ricompense
* Esperienza
* Oro
* Oggetti

### Tracking
Il tracking delle quest viene salvato nel database. Al completamento, il backend eroga le ricompense.

---

## 22. Sistema Mercanti

Funzionalità:
* Acquisto oggetti
* Vendita oggetti

I prezzi vengono definiti da database.

---

## 23. Locande e Checkpoint

Ogni villaggio possiede una locanda, che funge da checkpoint attivo.

Interagendo con una locanda, viene aggiornato il checkpoint del personaggio.

### Alla morte
* Respawn presso l'ultima locanda visitata.
* Penalità: perdita del 10% dell'oro posseduto.
* L'equipaggiamento viene mantenuto (nessuna perdita di progressi o oggetti).

---

## 24. Sistema di Salvataggio

Salvataggio automatico fuori dal combattimento.

### Eventi che attivano il salvataggio
* Cambio zona
* Completamento quest
* Visita locanda
* Spostamento o azione rilevante in esplorazione
* Intervallo periodico

### Dati salvati
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

## 25. API Backend

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

## 26. MVP Iniziale

Contenuti minimi richiesti:

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
* Persistenza completa tramite database

Il progetto dovrà essere sviluppato in modo modulare e facilmente estendibile, per consentire l'aggiunta futura di nuove classi, zone, boss, quest, abilità e sistemi di gioco senza modifiche sostanziali all'architettura esistente.

---

## 27. Requisiti Non Funzionali e Qualità

* **Carico:** ottimizzato per singolo utente (carico minimo sul database).
* **Documentazione:** autodocumentazione delle API tramite **Swagger** abilitato in ambiente di sviluppo.
* **Stabilità:** suite di unit test integrata sul backend per validare le formule di danno, l'assegnazione dell'esperienza e i passaggi di livello.