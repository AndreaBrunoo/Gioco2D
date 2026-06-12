using System.Text.Json;
using GiocoV1.Configurazioni;
using GiocoV1.Modelli;

namespace GiocoV1.Servizi;

public static class ServizioNemici
{
    // ---------------------------------------------------------
    // CARICA NEMICI DAL JSON
    // ---------------------------------------------------------
    public static ConfigNemici CaricaNemici(string percorso)
    {
        if (!File.Exists(percorso))
            throw new FileNotFoundException($"File nemici non trovato: {percorso}");

        string json = File.ReadAllText(percorso);

        var opzioni = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var config = JsonSerializer.Deserialize<ConfigNemici>(json, opzioni);

        if (config == null)
            throw new Exception("Errore nel parsing del JSON dei nemici!");

        return config;
    }

    // ---------------------------------------------------------
    // ASSEGNA LE MOSSE AI NEMICI E AI BOSS
    // ---------------------------------------------------------
    public static void AssegnaMosse(ConfigNemici configurazioneNemici)
    {
        foreach (var sezione in configurazioneNemici.Sezioni.Values)
        {
            // NEMICI NORMALI
            foreach (var nemico in sezione.Nemici)
            {
                nemico.Mosse = new List<Mossa>();

                foreach (var nome in nemico.NomiMosse)
                {
                    var mossa = ServizioMosse.OttieniMossaTramiteNome(nome);
                    if (mossa != null)
                        nemico.Mosse.Add(mossa);
                }
            }

            // BOSS
            if (sezione.Boss != null)
            {
                sezione.Boss.Mosse = new List<Mossa>();

                foreach (var nome in sezione.Boss.NomiMosse)
                {
                    var mossa = ServizioMosse.OttieniMossaTramiteNome(nome);
                    if (mossa != null)
                        sezione.Boss.Mosse.Add(mossa);
                }
            }
        }
    }

    // ---------------------------------------------------------
    // ASSEGNA GLI OGGETTI AI NEMICI E AI BOSS
    // ---------------------------------------------------------
    public static void AssegnaOggetti(ConfigNemici configurazioneNemici)
    {
        foreach (var sezione in configurazioneNemici.Sezioni.Values)
        {
            // NEMICI NORMALI
            foreach (var nemico in sezione.Nemici)
            {
                nemico.Inventario = new List<OggettoInventario>();

                foreach (var item in nemico.NomiOggetti)
                {
                    var oggetto = ServizioOggetti.OttieniOggettoTramiteNome(item.Nome);
                    if (oggetto != null)
                    {
                        nemico.Inventario.Add(new OggettoInventario
                        {
                            Oggetto = oggetto,
                            Quantita = item.Quantita
                        });
                    }
                }
            }

            // BOSS
            if (sezione.Boss != null)
            {
                var boss = sezione.Boss;
                boss.Inventario = new List<OggettoInventario>();

                foreach (var item in boss.NomiOggetti)
                {
                    var oggetto = ServizioOggetti.OttieniOggettoTramiteNome(item.Nome);
                    if (oggetto != null)
                    {
                        boss.Inventario.Add(new OggettoInventario
                        {
                            Oggetto = oggetto,
                            Quantita = item.Quantita
                        });
                    }
                }
            }
        }
    }

    // ---------------------------------------------------------
    // TENTA LO SPAWN DI UN NEMICO
    // ---------------------------------------------------------
    public static Entita? TentaSpawnNemico(
        Sezione sezione,
        CellaPosizionata cella,
        ConfigNemici configurazioneNemici)
    {
        if (!configurazioneNemici.Sezioni.TryGetValue(sezione.Nome, out var configurazione))
            return null;

        // 1) BOSS
        if (configurazione.Boss != null && !configurazione.Boss.Sconfitto)
        {
            foreach (var celleBoss in configurazione.CelleBoss)
            {
                if (celleBoss.X == cella.X && celleBoss.Y == cella.Y)
                    return ClonaBoss(configurazione.Boss);
            }
        }

        // 2) NEMICI NORMALI
        Random random = new();

        int tiroSpawnGenerale = random.Next(1, 101);
        if (tiroSpawnGenerale > configurazione.ProbabilitaSpawn)
            return null;

        int totaleProbabilità = configurazione.Nemici.Sum(n => n.ProbabilitaSpawn);
        int tiroSpawnNemico = random.Next(1, totaleProbabilità + 1);

        int intervallo = 0;
        foreach (var nemico in configurazione.Nemici)
        {
            intervallo += nemico.ProbabilitaSpawn;
            if (tiroSpawnNemico <= intervallo)
                return ClonaNemico(nemico);
        }

        return null;
    }

    // ---------------------------------------------------------
    // CLONA NEMICO
    // ---------------------------------------------------------
    private static Nemico ClonaNemico(Nemico nemico)
    {
        return new Nemico
        {
            Nome = nemico.Nome,
            SaluteMassima = nemico.SaluteMassima,
            SaluteAttuale = nemico.SaluteMassima,
            Attacco = nemico.Attacco,
            Difesa = nemico.Difesa,
            Velocita = nemico.Velocita,
            Livello = nemico.Livello,
            ProbabilitaSpawn = nemico.ProbabilitaSpawn,
            Mosse = new List<Mossa>(nemico.Mosse),
            Inventario = new List<OggettoInventario>(nemico.Inventario)
        };
    }

    // ---------------------------------------------------------
    // CLONA BOSS
    // ---------------------------------------------------------
    private static Boss ClonaBoss(Boss boss)
    {
        return new Boss
        {
            Nome = boss.Nome,
            Salute = boss.Salute,
            Attacco = boss.Attacco,
            Difesa = boss.Difesa,
            Velocita = boss.Velocita,
            Livello = boss.Livello,
            Mosse = new List<Mossa>(boss.Mosse),
            Inventario = new List<OggettoInventario>(boss.Inventario)
        };
    }
}