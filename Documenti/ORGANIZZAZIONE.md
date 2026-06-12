[Pagina precedente.](INIZIO.md)
# ORGANIZZAZIONE DEL PROGETTO

Questo documento definisce le regole operative per contribuire al progetto, mantenere ordine nel codice e garantire una collaborazione efficace tra tutti gli sviluppatori.

---

## 🧪 Test dopo ogni implementazione

Ogni volta che viene completata una task o una nuova funzionalità è **obbligatorio**:

1. **Testare localmente** la modifica  
   - Verificare che non introduca errori  
   - Controllare che non rompa funzionalità già esistenti  
   - Assicurarsi che il comportamento sia coerente con il resto del gioco  

2. **Aggiornare il file [Novità.](NOVITA.md)**  
   - Scrivere cosa è stato aggiunto, modificato o corretto  
   - Indicare eventuali note utili per gli altri sviluppatori  

Questo permette a tutti di sapere cosa è cambiato senza dover leggere commit o confrontare file.

---

## 💡 Idee, suggerimenti e discussioni

Per nuove idee, miglioramenti, riflessioni o funzionalità future utilizzare il file:

### **[Proposte.](PROPOSTE.md)**

Qui è possibile:

- proporre nuove meccaniche di gioco  
- discutere miglioramenti all’architettura  
- segnalare problemi non urgenti  
- annotare idee da valutare più avanti  

Questo file serve per mantenere traccia delle idee senza perderle nelle chat o nei commit.

---

## 📘 Informazioni tecniche sul codice

Per dettagli specifici su:

- funzionamento dei servizi  
- struttura dei modelli  
- logica interna di combattimento, inventario, mappa  
- convenzioni di scrittura del codice  
- spiegazioni tecniche approfondite  

utilizzare il file:

### **[Informazioni.](INFORMAZIONI.md)**

Questo file funge da documentazione tecnica interna, utile soprattutto per chi entra nel progetto per la prima volta.

---

## 📋 Gestione delle task

Le task da svolgere, in corso o completate, sono raccolte nel file:

### **[Task.](TASK.md)**

Qui ogni sviluppatore può:

- vedere cosa c’è da fare  
- prendere in carico una task  
- segnare una task come completata  
- aggiungere nuove attività se necessario  

È importante mantenere questo file aggiornato per evitare sovrapposizioni o lavori duplicati.

---

## 🌿 Convenzione per i branch

Ogni nuova implementazione deve essere sviluppata in un branch dedicato.

Regole:

- tutto minuscolo  
- parole separate da `-`  
- nome breve ma descrittivo  
- un branch = una singola funzionalità  

Esempi:

- `menu-inventario`  
- `drop-oggetti`  
- `equipaggiamento-arma`  
- `servizio-mappa`  

---

## 🔄 Flusso di lavoro consigliato

Eseguire il pull prima di ogni task.
1. Scegli una task da [Task.](TASK.md)  
2. Crea un branch dedicato  
3. Implementa la funzionalità  
4. Testa tutto accuratamente  
5. Aggiorna [Novità.](NOVITA.md)  
6. Se necessario, aggiorna [Informazioni.](INFORMAZIONI.md)  
7. Se hai idee o dubbi, scrivili in [Proposte.](PROPOSTE.md)  
8. Apri una pull request o segnala che il branch è pronto  

---

## 🎯 Obiettivo

L’obiettivo di questa organizzazione è:

- mantenere il progetto **pulito e comprensibile**  
- facilitare l’ingresso di nuovi sviluppatori  
- evitare conflitti e duplicazioni  
- documentare ogni passo in modo chiaro  