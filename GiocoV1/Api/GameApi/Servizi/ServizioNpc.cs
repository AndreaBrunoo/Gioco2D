using System.Text.Json;
using GiocoV1.Configurazioni;
using GiocoV1.Modelli;

namespace GiocoV1.Servizi;

public static class ServizioNpc
{
    public static ConfigNpc CaricaNpc(string percorso)
    {
        if (!File.Exists(percorso))
            throw new FileNotFoundException($"File NPC non trovato: {percorso}");

        string json = File.ReadAllText(percorso);

        var opzioni = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var config = JsonSerializer.Deserialize<ConfigNpc>(json, opzioni);

        // Se il JSON è vuoto o non mappabile, ritorna una configurazione vuota (NPC opzionali)
        if (config == null)
            return new ConfigNpc();

        return config;
    }

    public static Npc? OttieniNpcInCella(Sezione sezione, CellaPosizionata cella, ConfigNpc configurazioneNpc)
    {
        if (!configurazioneNpc.Sezioni.TryGetValue(sezione.Nome, out var configurazione))
            return null;

        foreach (var cellaNpc in configurazione.CelleNpc)
        {
            if (cellaNpc.X == cella.X && cellaNpc.Y == cella.Y)
            {
                var npc = configurazione.Npc.FirstOrDefault(n => n.Nome == cellaNpc.Nome);
                if (npc != null)
                    return ClonaNpc(npc);
            }
        }

        return null;
    }

    private static Npc ClonaNpc(Npc npc)
    {
        return new Npc
        {
            Nome = npc.Nome,
            PosX = npc.PosX,
            PosY = npc.PosY,
            Dialoghi = new List<string>(npc.Dialoghi),
            Interagibile = npc.Interagibile,
            Ruolo = npc.Ruolo
        };
    }
}
