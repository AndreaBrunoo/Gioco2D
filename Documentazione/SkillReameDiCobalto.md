# Gioco2D – Progetto Tecnico & Game Design Document

Benvenuto Agente AI. Questo documento riassume l'architettura, le regole di design e le meccaniche core di **Gioco2D**, un RPG tattico 2D single-player a turni, per permetterti di operare immediatamente sul codice sorgente e sulla logica di gioco.

---

## 1. Stack Tecnologico & Architettura

Il progetto segue un approccio **autoritativo lato server (anti-cheat)**. Il client propone le azioni, il server le convalida e ne decreta l'esito globale.

*   **Backend (C# .NET Web API):** 
    *   **Architettura:** Clean Architecture / CQRS con MediatR. Controller snelli; logica di calcolo, IA e formule interamente delegata a Servizi o Entità di Dominio.
    *   **Persistenza:** SQLite gestito tramite Entity Framework Core (Fluent API per le configurazioni).
    *   **Stato del Combattimento:** Interamente **in-memory** (es. `IMemoryCache` o ConcurrentDictionary keyed by `UserId`). Non viene persistito su DB. Al riavvio del server o al re-login, il match in corso è perso e il giocatore riparte dall'ultimo salvataggio fuori combattimento.
    *   **Sicurezza:** Autenticazione JWT tramite header `Authorization: Bearer <token>`. Tipi ID Fortemente Tipizzati (Strongly Typed IDs, es. `record EnemyId(Guid Value)`).
*   **Frontend (Angular 21):**
    *   **Paradigma:** Standalone Components, Lazy Loading.
    *   **Gestione di Stato:** Uso esclusivo dei nuovi **Signals** (`signal`, `computed`, `effect`) per la reattività della UI, statistiche e stato della griglia. RxJS limitato alle richieste HTTP.
    *   **Rendering Visivo:** HTML5 Canvas ottimizzato per la griglia di esplorazione e combattimento, evitando manipolazioni dirette e pesanti del DOM. Ridisegna solo le porzioni di griglia modificate.
*   **Comunicazione BE/FE:**
    *   Endpoint RESTful sotto `/api/{modulo}`.
    *   **Pattern Azione-Risposta Singola nei turni:** Il FE invia l'azione (es. muovi, attacca), il BE la valida, calcola ed esegue immediatamente la contromossa dell'I.A. nemica e risponde con un singolo payload contenente l'esito del giocatore + la mossa dell'I.A. + lo stato aggiornato.

---

## 2. Regole di Game Design Core

### Classi & Statistiche
*   **Classi Giocabili:** Guerriero (HP/Difesa alti, mischia), Arciere (distanza, mobilità, critico alto), Mago (danno magico AoE, consuma mana).
*   **5 Statistiche Core:** HP, MP, Forza, Intelligenza, Destrezza.
*   **Attributi Derivati:** Difesa (riduzione danno fisico), Velocità (ordine dei turni e capacità di movimento).
*   **Progressione:** Level Cap 100, curva EXP esponenziale. Punti Abilità liberi spendibili sulle statistiche core a ogni level up.

### Struttura del Mondo: Il Reame Sotterraneo di Cobalto
L'ambientazione selezionata è una colonna mineraria sotterranea claustrofobica suddivisa in **5 zone MVP** sequenziali (Griglia standard 12x12 per mappa):
1.  **Zona 1 (Liv. 1-5) – Avamposto dei Minatori:** Zona sicura (Villaggio) che funge da Hub con Locanda/Checkpoint, mercanti e NPC, parzialmente esposta alla luce superficiale.
2.  **Zona 2 (Liv. 6-10) – Le Gallerie Abbandonate:** Binari interrotti e cunicoli stretti.
3.  **Zona 3 (Liv. 11-15) – I Funghi Luminescenti:** Vegetazione parassitaria che genera illuminazione soffusa ciano/verde, ma rilascia spore velenose.
4.  **Zona 4 (Liv. 16-20) – La Città di Pietra Morta:** Rovine geometriche nane e buio pesto.
5.  **Zona 5 (Liv. 21-25) – Il Cuore di Cristallo:** Faglia geomorfica finale ricca di cristalli riflettenti.

### Salvataggi e Morte
*   **Salvataggio Automatico:** Eseguito dal BE fuori combattimento al cambio zona, completamento quest, riposo alla locanda o intervalli periodici.
*   **Morte:** Respawn all'ultima locanda dell'Avamposto, detrazione del 10% dell'oro attuale, nessuna perdita di equipaggiamento o progressi di livello.

---

## 3. Meccaniche Avanzate Implementate

### A. La Meccanica del Buio (Visibility & Line of Sight)
L'oscurità è un fattore tattico determinante sia in esplorazione che nel combat loop.
*   **Raggio Visivo Base:** Nelle stanze buie o nei livelli profondi, il personaggio ha una visibilità limitata a sole **2 celle** di raggio.
*   **La Torcia:** Equipaggiando l'oggetto Torcia (che occupa attivamente uno slot mano, es. `OffHand` o `MainHand`, sacrificando scudi o armi a due mani) il raggio visivo aumenta di **+4 celle** (totale 6 celle).
*   **Validazione Server (LoS):** Il server calcola le celle visibili tramite algoritmi di prossimità (es. distanza Euclidea o Chebyshev). Se un nemico si trova nel buio fuori dal raggio visivo, il client non può bersagliarlo con attacchi single-target o magie.
*   **Rendering Canvas:** Il client applica una maschera di oscurità (celle nere o effetto vignetta/gradient) basandosi sui Signals reattivi di posizione e raggio visivo calcolati/convalidati dal server.

### B. L'Ascensore Centrale (Deep Lift) & Esplorazione Non-Lineare
Una meccanica ad alto rischio e alta ricompensa (*High Risk, High Reward*) che rompe la linearità dei livelli.
*   **Funzionamento:** Situato al centro di ogni mappa, l'ascensore permette di viaggiare liberamente verso **qualsiasi piano (Zona 1-5) in qualunque momento**, senza barriere o hard gate legati al livello del personaggio.
*   **Il Tutorial di Bram:** L'unica condizione di sblocco dell'ascensore è il completamento del tutorial iniziale nella Zona 1, dove l'NPC *Bram il Capomastro* spiega le basi del combattimento e mostra il funzionamento del lift. Il completamento imposta il flag `IsTutorialCompleted = true` sul DB.
*   **Interfaccia Signals:** Il frontend consuma un `LiftService` e mostra tramite componenti standalone i livelli consigliati di ogni piano. Se un giocatore di livello basso tenta di scendere al Piano 5, Angular genera un avviso visivo di *Pericolo Elevato*, ma non blocca l'azione.
*   **Validazione di Stato:** L'endpoint `/api/movement/use-open-lift` rifiuta la transizione di zona se l'ID utente risulta associato a un combattimento attivo in memoria (`IMemoryCache`).

---

## 4. NPC, Nemici Comuni e Boss Chiave per Zona

Ogni zona presenta un ecosistema unico progettato attorno alle meccaniche di luce/buio. Per ogni area, oltre al Boss e all'NPC, sono presenti due tipologie di nemici standard: un'unità da mischia e un'unità a distanza (Ranged).

### Zona 1 – Avamposto dei Minatori (Liv. 1-5)
La luce naturale filtra debolmente. I nemici qui introducono il giocatore alle meccaniche base di adiacenza e posizionamento.
*   **NPC Unico:** *Bram il Capomastro* (Quest Giver & Gestore Ascensore). Spiega le basi del combattimento.
*   **Nemico Mischia: Parassita Scavatore (Liv. 1-3)**
    *   *Meccanica:* Ha una Velocità elevata ma pochissimi HP. Tenta di aggirare il giocatore sulla griglia per attaccare alle spalle (ottiene +20% precisione se attacca da dietro).
*   **Nemico Ranged: Pipistrello Sputatore (Liv. 2-4)**
    *   *Meccanica:* Gittata 3 celle. Sputa acido minerale a bassa precisione. Se il giocatore si trova su una cella illuminata dai pozzi di luce, il Pipistrello ha uno svantaggio al tiro mirato a causa del riverbero.
*   **Boss:** *Grinfiamarcio, il Topo dei Cunicoli (Liv. 5).* Subisce malus alla difesa se trascinato o guidato vicino alle celle illuminate dai bracieri.

### Zona 2 – Le Gallerie Abbandonate (Liv. 6-10)
Qui inizia il buio totale. I nemici sfruttano attivamente la presenza o l'assenza della torcia.
*   **NPC Unico:** *Flint il Lanternaio* (Mercante di torce e olio).
*   **Nemico Mischia: Ladro dei Cunicoli (Liv. 6-8)**
    *   *Meccanica: "Sabotaggio".* Sbuca dal buio fuori dal raggio visivo. Se riesce a colpire il giocatore in mischia, c'è una probabilità del 30% che "spenga" temporaneamente la torcia per 1 turno, riducendo il raggio visivo del giocatore a 2 celle.
*   **Nemico Ranged: Goblin Fiondatore (Liv. 7-9)**
    *   *Meccanica: "Cecchino Cieco".* Rimane nell'oscurità e scaglia pietre contro il giocatore. Può mirare solo alle celle illuminate (ovvero dove si trova il giocatore con la torcia accesa). Se il giocatore spegne la torcia o si sposta nel buio, il Goblin perde il bersaglio e attacca a caso sulla griglia.
*   **Boss:** *Il Custode Meccanico Difettoso (Liv. 10).* Automa cieco; rileva il giocatore solo se quest'ultimo lo illumina con la propria torcia o se attacca.

### Zona 3 – I Funghi Luminescenti (Liv. 11-15)
Luce soffusa ciano/verde. Le meccaniche ruotano attorno ad alterazioni di stato e manipolazione delle celle.
*   **NPC Unico:** *Myra l'Erborista Cieca* (Scambia spore speciali per pozioni).
*   **Nemico Mischia: Coleottero Corazzato (Liv. 11-13)**
    *   *Meccanica: "Carica Tossica".* Si muove solo in linea retta sulla griglia. Se attraversa una cella contenente un fungo luminescente, lo calpesta distruggendolo, rimuovendo la luce da quella cella e rilasciando una nube tossica AoE (3x3) che infligge lo status `Veleno`.
*   **Nemico Ranged: Spora Tossica Volante (Liv. 12-14)**
    *   *Meccanica: "Impollinazione".* Non infligge danno diretto elevato, ma spara proiettili a parabola (gittata 4, ignora gli ostacoli bassi) che applicano lo status `Rallentamento`, riducendo il movimento sulla griglia del giocatore del 50%.
*   **Boss:** *Spore-Goliath, la Micosi Ruggente (Liv. 15).* Statico, riduce temporaneamente il raggio visivo del giocatore a 1 cella tramite polline; debole ai bulbi luminosi esplosivi sulla griglia.

### Zona 4 – La Città di Pietra Morta (Liv. 16-20)
Buio pesto e architettura nanica. I nemici qui sono letali se affrontati senza una fonte di luce efficiente.
*   **NPC Unico:** *L'Ombra Sibilante* (Spettro che svanisce se illuminato; per parlarci bisogna disequipaggiare la torcia).
*   **Nemico Mischia: Ombra Ghermitrice (Liv. 16-18)**
    *   *Meccanica: "Fuga dalla Luce".* Se l'Ombra inizia il suo turno all'interno del raggio di luce della torcia del giocatore, subisce piccoli danni da scottatura e la sua Velocità è dimezzata. Se si trova nel buio, ottiene +2 celle di movimento e può attraversare gli ostacoli della griglia.
*   **Nemico Ranged: Arciere Spettrale Nano (Liv. 17-19)**
    *   *Meccanica: "Frecce Perforanti".* Spara frecce d'ombra a lunga gittata (5 celle). Le frecce infliggono danno magico (basato su Intelligenza) anziché fisico e riducono i MP del giocatore se colpiscono mentre quest'ultimo si trova in una cella completamente buia.
*   **Boss:** *Malakor, il Boia d'Ombra (Liv. 20).* Invisibile e non bersagliabile finché si trova nelle celle buie, infligge critici se attacca dall'oscurità.

### Zona 5 – Il Cuore di Cristallo (Liv. 21-25)
Luce riflessa e geometrica. I nemici interagiscono con le proprietà ottiche e i coni d'ombra dei cristalli di Cobalto.
*   **NPC Unico:** *L'Esploratore Coraggioso* (Superstite/Mercante Endgame).
*   **Nemico Mischia: Segugio di Cobalto (Liv. 21-23)**
    *   *Meccanica: "Balzo dall'Ombra".* Se il segugio si trova in una cella di oscurità adiacente a un cono di luce generato dai cristalli, può eseguire un balzo di 3 celle per colpire istantaneamente il giocatore, infliggendo lo status `Stordimento` (salta il turno successivo).
*   **Nemico Ranged: Geode Vivente (Liv. 22-24)**
    *   *Meccanica: "Lente di Rifrazione".* Questo nemico cristallino spara un raggio laser in linea retta. Se il raggio colpisce uno dei cristalli di Cobalto presenti sulla griglia, **il laser rimbalza a 90 gradi**, permettendo al Geode di colpire il giocatore anche se quest'ultimo si trova riparato dietro un angolo o un muro.
*   **Boss:** *Il Terrore Cieco dell'Abisso (Liv. 25).* Usa l'ecolocalizzazione; ignora il buio e ottiene bonus se il giocatore consuma MP o corre. Può essere accecato riflettendo la luce della torcia sui cristalli.

---

## 5. Comandi Rapidi Disponibili nella Skill

Usa questi comandi per generare o revisionare il codice mantenendo intatta la coerenza architetturale:
*   `/scaffold [nome_modulo]` -> Genera entità C#, Strongly Typed IDs, repository, controller REST + Standalone Components e Services con Signals in Angular 21.
*   `/logica [meccanica]` -> Sviluppa algoritmi (es. raggio visivo, calcolo danni, IA dei boss ciechi).
*   `/db [entità]` -> Genera schemi EF Core e configurazioni Fluent API.
*   `/review` -> Ispeziona il codice alla ricerca di bug di performance, memory leak nel combat loop in memoria, o violazioni dell'approccio autoritativo del server.