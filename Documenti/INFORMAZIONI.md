[Pagina precedente.](ORGANIZZAZIONE.md)

# INFORMAZIONI TECNICHE

Questo documento raccoglie informazioni tecniche pratiche sul progetto: struttura, modelli, servizi, configurazioni e punti importanti per chi vuole leggere o estendere il codice.

---

# 📦 Struttura generale del codice

Cartelle principali:

- `GiocoV1/` — codice sorgente e dati di gioco.
- `Modelli/` — classi che rappresentano entità di gioco (personaggi, nemici, oggetti, mappa).
- `Servizi/` — logica applicativa (movimento, incontri, caricamento, salvataggi, ecc.).
- `Configurazioni/` — classi che mappano i JSON esterni.
- `Dtos/` — strutture leggere per trasferimento dati.
- `Enum/` — enumerazioni usate nel progetto.
- `Salvataggi/` — file di salvataggio in JSON.

---

# 📐 Diagramma del progetto

Diagramma semplificato delle relazioni principali (mermaid):

```mermaid
    flowchart TD
		A[Program.cs] -->|carica| B[Configurazioni JSON]
		B --> C[ServizioMosse]
		B --> D[ServizioNemici]
		B --> E[ServizioOggetti]
		A --> F[ServizioMappa]
		F --> G[Modelli: Mappa / Sezione / Cella]
		A --> H[ServizioSalvataggio]
		Runtime((Esecuzione)) --> I[Servizi di gioco]
		I --> J[ServizioMovimento]
		I --> K[ServizioIncontri]
		I --> L[ServizioDrop]
		I --> M[ServizioOggetti]
		I --> N[ServizioMosse]
		I --> O[ServizioNemici]
		J --> G
		K --> G
		K --> P[Modelli: Personaggio / Nemico / Boss]
		M --> Q[Modelli: Oggetto / Equipaggiamento]
		H --> Salvataggi[Salvataggi/ (JSON)]
```

---

# 🧱 Modelli (sintesi)

Per ciascuna classe di Modelli elenco le proprietà (solo tipo + nome). I link puntano ai file sorgente relativi.

- [Entita.cs](../GiocoV1/Modelli/Entita.cs)
- `Entita`
    - `string Nome`
    - `int PosX`
    - `int PosY`

- [Personaggio.cs](../GiocoV1/Modelli/Personaggio.cs)
- `Personaggio`
  - `string Nome`
  - `int SaluteMassima`
  - `int SaluteAttuale`
  - `int Attacco`
  - `int Difesa`
  - `int Velocita`
  - `int Livello`
  - `int Esperienza`
  - `List<Mossa> Mosse`
  - `List<OggettoInventario> Inventario`
  - `Equipaggiamento Equipaggiamenti`
  - `int Monete`
  - `int PuntiAbilita`
  - `int PosX`
  - `int PosY`

- [Nemico.cs](../GiocoV1/Modelli/Nemico.cs)
- `Nemico : Entita`
	- `int SaluteAttuale`
	- `int SaluteMassima`
	- `int Attacco`
	- `int Difesa`
	- `int Velocita`
	- `int Livello`
	- `int ProbabilitaSpawn`
	- `List<string> NomiMosse`
	- `List<Mossa> Mosse`
	- `List<OggettoInventario> Inventario`
	- `List<OggettoInventarioJson> NomiOggetti`

- [Boss.cs](../GiocoV1/Modelli/Boss.cs)
- `Boss : Entita`
	- `int Salute`
	- `int Attacco`
	- `int Difesa`
	- `int Velocita`
	- `int Livello`
	- `List<string> NomiMosse`
	- `List<Mossa> Mosse`
	- `List<OggettoInventario> Inventario`
	- `List<OggettoInventarioJson> NomiOggetti`
	- `bool Sconfitto`

- [Mappa.cs](../GiocoV1/Modelli/Mappa.cs)
- `Mappa`
	- `string Nome`
	- `List<Sezione> Sezioni`

- [Sezione.cs](../GiocoV1/Modelli/Sezione.cs)
- `Sezione`
	- `string Nome`
	- `int Larghezza`
	- `int Altezza`
	- `List<CellaPosizionata> Celle`
	- `List<Entita> EntitaPresenti`

- [CellaPosizionata.cs](../GiocoV1/Modelli/CellaPosizionata.cs)
- `CellaPosizionata`
	- `int X`
	- `int Y`
	- `string Nome`
	- `string Descrizione`
	- `CollegamentoCella? CollegaA`

- [Cella.cs](../GiocoV1/Modelli/Cella.cs)
- `Cella`
	- `string Nome`
	- `string Descrizione`
	- `CollegamentoCella? CollegaA`

- [Mossa.cs](../GiocoV1/Modelli/Mossa.cs)
- `Mossa`
	- `string Nome`
	- `int PotenzaBase`
	- `int PrecisioneBase`
	- `int ProbabilitaCritico`

- [Oggetto.cs](../GiocoV1/Modelli/Oggetto.cs)
- `Oggetto`
	- `string Nome`
	- `int BonusAttacco`
	- `int BonusDifesa`
	- `int BonusVelocita`
	- `int BonusSalute`
	- `int ProbabilitaDrop`
	- `CategoriaOggetto Categoria`

- [OggettoInventario.cs](../GiocoV1/Modelli/OggettoInventario.cs)
- `OggettoInventario`
	- `Oggetto Oggetto`
	- `int Quantita`

- [Equipaggiamento.cs](../GiocoV1/Modelli/Equipaggiamento.cs)
- `Equipaggiamento`
	- `Mossa?[] MosseEquipaggiate`
	- `OggettoInventario? Arma`
	- `OggettoInventario? Elmo`
	- `OggettoInventario? Corazza`
	- `OggettoInventario? Gambali`
	- `OggettoInventario? Stivali`
	- metodi: `Get(CategoriaOggetto)`, `Set(CategoriaOggetto, OggettoInventario?)`

- [ClassePersonaggio.cs](../GiocoV1/Modelli/ClassePersonaggio.cs)
- `ClassePersonaggio`
	- `string Nome`
	- `int Salute`
	- `int Attacco`
	- `int Difesa`
	- `int Velocita`

- [StatoGioco.cs](../GiocoV1/Modelli/StatoGioco.cs)

- [Npc.cs](../GiocoV1/Modelli/Npc.cs)
- `Npc : Entita`
	- `List<string> Dialoghi`
	- `bool Interagibile`
	- `string Ruolo`
- `StatoGioco`
	- `string VersioneGioco`
	- `DateTime Timestamp`
	- `Personaggio Personaggio`
	- `string AreaCorrente`
	- `int PosizioneX`
	- `int PosizioneY`
	- `HashSet<string> BossSconfitti`
	- `HashSet<string> OggettiRaccolti`
	- `HashSet<string> NemiciEliminati`
	- `HashSet<string> EventiCompletati`

---

# ⚙️ Servizi (sintesi)

Per ogni servizio elenco i metodi pubblici principali e i parametri richiesti (solo nome e tipo). I link puntano ai file in `GiocoV1/Servizi` o ai modelli usati.

- [ServizioMappa.cs](../GiocoV1/Servizi/ServizioMappa.cs)
- `ServizioMappa` (static)
	- `Mappa CaricaMappa(string percorso)` — legge JSON e deserializza in `Mappa`.
	- `Cella?[,] CreaGriglia(Sezione sezione)` — costruisce la griglia 2D di `Cella` da `Sezione`.

- [ServizioMovimento.cs](../GiocoV1/Servizi/ServizioMovimento.cs)
- `ServizioMovimento`
	- costruttore: `ServizioMovimento(Cella?[,] griglia)` — richiede la griglia corrente.
	- `RisultatoMovimento Muovi(Personaggio personaggio, char direzione, Sezione sezione, ConfigNemici configNemici)` — muove il personaggio e può avviare spawn nemici.

- [ServizioIncontri.cs](../GiocoV1/Servizi/ServizioIncontri.cs)
- `ServizioIncontri`
	- `EsitoIncontro Incontro(Personaggio personaggio, Entita entita)` — logica principale del combattimento.
	- `void EseguiAttacco(object attaccante, object bersaglio, Mossa mossa)` — applica un attacco.
	- `int CalcolaDanno(object attaccante, object bersaglio, Mossa mossa)` — restituisce danno calcolato.
	- metodi helper: `ScegliMossaNemico(Nemico)`, `TentaFuga(Personaggio,Nemico)`, disegno UI/HP bar.

- [ServizioNemici.cs](../GiocoV1/Servizi/ServizioNemici.cs)
- `ServizioNemici`
	- `ConfigNemici CaricaNemici(string percorso)` — deserializza JSON nemici.
	- `void AssegnaMosse(ConfigNemici configurazioneNemici)` — assegna oggetti `Mossa` ai nemici.
	- `void AssegnaOggetti(ConfigNemici configurazioneNemici)` — assegna inventari/oggetti.
	- `Entita? TentaSpawnNemico(Sezione sezione, CellaPosizionata cella, ConfigNemici configurazioneNemici)` — tenta spawn in una cella.

- [ServizioMosse.cs](../GiocoV1/Servizi/ServizioMosse.cs)
- `ServizioMosse`
	- `void CaricaMosse(string percorso)` — carica mosse dai JSON in memoria.
	- `void AggiungiMossaAlPersonaggio(Personaggio personaggio, Mossa nuovaMossa)`
	- `void EquipaggiaMossa(Personaggio personaggio, Mossa mossaDaEquipaggiare, int slot)`
	- `void RimuoviMossaEquipaggiata(Personaggio personaggio, int slot)`
	- `Mossa? OttieniMossaTramiteNome(string nome)`

- [ServizioOggetti.cs](../GiocoV1/Servizi/ServizioOggetti.cs)
- `ServizioOggetti`
	- `void CaricaOggettiPerCategoria(string percorso)`
	- `void CaricaTuttiOggetti()`
	- `void AggiungiOggettoAlPersonaggio(Personaggio personaggio, OggettoInventario nuovoOggetto)`
	- `void EquipaggiaOggetto(Personaggio personaggio, OggettoInventario nuovoOggetto)`
	- `void EquipaggiaOggettoDaNome(Personaggio personaggio, string nomeOggetto)`
	- `void RimuoviOggettoEquipaggiato(Personaggio personaggio, OggettoInventario oggettoDaRimuovere)`
	- `Oggetto? OttieniOggettoTramiteNome(string nome)`

- [ServizioDrop.cs](../GiocoV1/Servizi/ServizioDrop.cs)
- `ServizioDrop`
	- `void ApplicaDrop(Personaggio personaggio, Nemico nemico, EsitoIncontro esito)` — calcola XP, monete e drop oggetti.
	- `int CalcolaXpPerLivello(int livello)`
	- `void ApplicaLevelUp(Personaggio personaggio)`

- [ServizioClassiPersonaggio.cs](../GiocoV1/Servizi/ServizioClassiPersonaggio.cs)

- [ServizioNpc.cs](../GiocoV1/Servizi/ServizioNpc.cs)
- `ServizioNpc`
	- `ConfigNpc CaricaNpc(string percorso)` — deserializza JSON npc.
	- `Npc? OttieniNpcInCella(Sezione sezione, CellaPosizionata cella, ConfigNpc configurazioneNpc)` — restituisce un NPC se configurato nella cella.
- `ServizioClassi` 
	- `List<ClassePersonaggio> ClassiDisponibili { get; }` — elenco classi predefinite.
	- `void ApplicaClasse(Personaggio p, ClassePersonaggio classe)`

- [ServizioSalvataggio.cs](../GiocoV1/Servizi/ServizioSalvataggio.cs)
- `ServizioSalvataggio`
	- costruttore: `ServizioSalvataggio()` — assicura cartella salvataggi.
	- `string SalvaNuovoSlot(StatoGioco stato)` — salva in nuovo file e ritorna il percorso.
	- `StatoGioco Carica(string percorso)` — deserializza uno slot.
	- `List<string> ElencaSalvataggi()` — ritorna i percorsi dei salvataggi.

---

# 🗂️ Configurazioni

Classi in `Configurazioni/` (mappano i JSON):

- [ConfigMosse.cs](../GiocoV1/Configurazioni/ConfigMosse.cs)
- `ConfigMosse` 
    - `List<Mossa> Mosse`

- [ConfigNemici.cs](../GiocoV1/Configurazioni/ConfigNemici.cs)
- `ConfigNemici` 
    - `Dictionary<string, ConfigSezioneNemici> Sezioni`
        - `ConfigSezioneNemici` 
            - `int ProbabilitaSpawn`
            - `List<Nemico> Nemici` 
            - `Boss? Boss`
            - `List<CellaBoss> CelleBoss`
                - `CellaBoss`
                    - `int X`
                    - `int Y`

- [ConfigOggetti.cs](../GiocoV1/Configurazioni/ConfigOggetti.cs)
- `ConfigOggetti` 
    - `List<Oggetto> Consumabili`
    - `List<Oggetto> Offensivi`
    - `List<Oggetto> Materiali`
    - `EquipaggiamentiConfig Equipaggiamenti` 
        - `EquipaggiamentiConfig`
        - `List<Oggetto> Elmi`
        - `List<Oggetto> Corazze`
        - `List<Oggetto> Gambali`
        - `List<Oggetto> Stivali`
        - `List<Oggetto> Armi`

- [ConfigSalvataggio.cs](../GiocoV1/Configurazioni/ConfigSalvataggio.cs)
- `ConfigSalvataggi`
    - `const string CartellaSalvataggi`
    - `const string Prefisso`
    - `const string Estensione`

---

# 🧾 DTOs (sintesi)

- [CollegamentoCella.cs](../GiocoV1/Dtos/CollegamentoCella.cs)
- `CollegamentoCella` 
    - `string Sezione`
    - `int X`
    - `int Y`

- [OggettoInventarioJson.cs](../GiocoV1/Dtos/OggettoInventarioJson.cs)
- `OggettoInventarioJson` 
    - `string Nome`
    - `int Quantita`

- [RisultatoMovimento.cs](../GiocoV1/Dtos/RisultatoMovimento.cs)
- `RisultatoMovimento` 
    - `string Messaggio1`
    - `string Descrizione`
    - `Entita? NemicoTrovato`
    - `CollegamentoCella? Collegamento`

---

# 🔢 Enum

- [CategoriaOggetto.cs](../GiocoV1/Enum/CategoriaOggetto.cs)
- `CategoriaOggetto` 
    - `Consumabile`
    - `Offensivo`
    - `Materiale`
    - `Elmo`
    - `Corazza`
    - `Gambali`
    - `Stivali`
    - `Arma`
    
- [EsitoIncontro.cs](../GiocoV1/Enum/EsitoIncontro.cs)
- `EsitoIncontro` 
    - `Vittoria`
    - `Sconfitta`
    - `Fuga`
    - `NessunIncontro`

---

# 🗂️ Dati esterni (JSON)

I file JSON in radice contengono i dati caricati all’avvio:

- [Mappa_principale.json](../GiocoV1/Mappa_principale.json) contiene l'intera mappa di gioco, seguire [Mappa.](../GiocoV1/Modelli/Mappa.cs)
- [Mosse.json](../GiocoV1/Mosse.json) contiene i dettagli delle mosse presenti nel gioco, seguire [ConfigMosse.](../GiocoV1/Configurazioni/ConfigMosse.cs)
- [Nemici.json](../GiocoV1/Nemici.json) contiene i nemici presenti nel gioco, seguire [ConfigNemici.](../GiocoV1/Configurazioni/ConfigNemici.cs)
- [Oggetti.json](../GiocoV1/Oggetti.json) contiene tutti gli oggetti del gioco, seguire [ConfigOggetti](../GiocoV1/Configurazioni/ConfigOggetti.cs)

- [npc.json](../GiocoV1/npc.json) contiene la configurazione degli NPC in mappa, seguire [ConfigNpc](../GiocoV1/Configurazioni/ConfigNpc.cs)

Questi file permettono il bilanciamento senza toccare codice.

---

# 🧩 Convenzioni del codice

### **1. Separazione logica**
Ogni funzionalità deve essere implementata nel servizio corretto.  
Esempio: non inserire logica di combattimento dentro il movimento.

### **2. Nomi chiari**
Classi, metodi e variabili devono essere leggibili coerenti e in italiano.

### **3. Evitare duplicazioni**
Se una logica viene ripetuta, spostarla in un metodo dedicato.

### **4. Dati esterni**
Tutto ciò che può essere configurato deve stare nei JSON.

---

# 📚 Collegamenti utili

- Modifiche recenti → `NOVITA.md`  
- Idee e discussioni → `PROPOSTE.md`  
- Task operative → `TASK.md`  
- Regole organizzative → `ORGANIZZAZIONE.md`  

---

# 🎯 Obiettivo del file

Questo documento serve a:

- fornire una panoramica tecnica chiara  
- aiutare nuovi sviluppatori a capire rapidamente il progetto  
- mantenere coerenza e qualità nel codice  
- evitare errori dovuti a mancanza di informazioni  