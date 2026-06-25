using System.Text.Json;
using GiocoV1.Modelli;
using GiocoV1.Configurazioni;

namespace GiocoV1.Servizi;

public class ServizioMosse
{
    private static List<Mossa> _mosse = new();

    public static void CaricaMosse(string percorso)
    {
        if (!File.Exists(percorso))
            throw new FileNotFoundException($"File mosse non trovato: {percorso}");

        string json = File.ReadAllText(percorso);

        var opzioni = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var config = JsonSerializer.Deserialize<ConfigMosse>(json, opzioni)
            ?? throw new Exception("Errore nel parsing del JSON delle mosse!");

        _mosse = config.Mosse;
    }

    public static void AggiungiMossaAlPersonaggio(Personaggio personaggio, Mossa nuovaMossa)
    {
        // Se c'è uno slot libero, equipaggia automaticamente
        for (int i = 0; i < personaggio.Equipaggiamenti.MosseEquipaggiate.Length; i++)
        {
            if (personaggio.Equipaggiamenti.MosseEquipaggiate[i] == null)
            {
                personaggio.Equipaggiamenti.MosseEquipaggiate[i] = nuovaMossa;
                return;
            }
        }

        // Altrimenti finisce nella lista delle mosse possedute
        personaggio.Mosse.Add(nuovaMossa);
    }

    public static void EquipaggiaMossa(Personaggio personaggio, Mossa mossaDaEquipaggiare, int slot)
    {
        if (personaggio.Equipaggiamenti.MosseEquipaggiate[slot] != null)
            personaggio.Mosse.Add(personaggio.Equipaggiamenti.MosseEquipaggiate[slot]!);

        personaggio.Equipaggiamenti.MosseEquipaggiate[slot] = mossaDaEquipaggiare;

        personaggio.Mosse.Remove(mossaDaEquipaggiare);
    }

    public static void RimuoviMossaEquipaggiata(Personaggio personaggio, int slot)
    {
        if (personaggio.Equipaggiamenti.MosseEquipaggiate[slot] != null)
            personaggio.Mosse.Add(personaggio.Equipaggiamenti.MosseEquipaggiate[slot]!);

        personaggio.Equipaggiamenti.MosseEquipaggiate[slot] = null;
    }
    
    public static Mossa? OttieniMossaTramiteNome(string nome)
    {
        return _mosse
            .FirstOrDefault(m =>
                m.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
    }
}