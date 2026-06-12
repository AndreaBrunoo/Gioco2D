using System.Text.Json;
using GiocoV1.Modelli;
using GiocoV1.Configurazioni;

namespace GiocoV1.Servizi;

public class ServizioSalvataggio
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public ServizioSalvataggio()
    {
        if (!Directory.Exists(ConfigSalvataggi.CartellaSalvataggi))
            Directory.CreateDirectory(ConfigSalvataggi.CartellaSalvataggi);
    }

    // 🔹 Genera automaticamente il nome del prossimo slot
    private string GeneraNomeFile()
    {
        var files = Directory.GetFiles(ConfigSalvataggi.CartellaSalvataggi, $"{ConfigSalvataggi.Prefisso}*{ConfigSalvataggi.Estensione}");
        int max = 0;

        foreach (var file in files)
        {
            var nome = Path.GetFileNameWithoutExtension(file);
            var numeroString = nome.Replace(ConfigSalvataggi.Prefisso, "");

            if (int.TryParse(numeroString, out int numero))
                if (numero > max)
                    max = numero;
        }

        int prossimo = max + 1;
        string nomeFile = $"{ConfigSalvataggi.Prefisso}{prossimo:000}{ConfigSalvataggi.Estensione}";
        return Path.Combine(ConfigSalvataggi.CartellaSalvataggi, nomeFile);
    }

    // 🔹 Salva in un nuovo slot
    public string SalvaNuovoSlot(StatoGioco stato)
    {
        stato.Timestamp = DateTime.Now;

        string percorso = GeneraNomeFile();
        var json = JsonSerializer.Serialize(stato, _options);
        File.WriteAllText(percorso, json);

        return percorso;
    }

    // 🔹 Carica uno slot specifico
    public StatoGioco Carica(string percorso)
    {
        if (!File.Exists(percorso))
            throw new FileNotFoundException("Salvataggio non trovato");

        var json = File.ReadAllText(percorso);
        return JsonSerializer.Deserialize<StatoGioco>(json)
               ?? throw new Exception("Errore nel caricamento");
    }

    // 🔹 Elenca tutti gli slot disponibili
    public List<string> ElencaSalvataggi()
    {
        return Directory.GetFiles(ConfigSalvataggi.CartellaSalvataggi, $"{ConfigSalvataggi.Prefisso}*{ConfigSalvataggi.Estensione}")
                        .OrderBy(f => f)
                        .ToList();
    }
}