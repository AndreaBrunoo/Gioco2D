namespace GiocoV1.Modelli;

public class StatoGioco
{
    public string VersioneGioco { get; set; } = "1.0.0";
    public DateTime Timestamp { get; set; } = DateTime.Now;

    public Personaggio Personaggio { get; set; } = new();

    public string AreaCorrente { get; set; } = string.Empty;
    public int PosizioneX { get; set; }
    public int PosizioneY { get; set; }

    public HashSet<string> BossSconfitti { get; set; } = new();
    public HashSet<string> OggettiRaccolti { get; set; } = new();
    public HashSet<string> NemiciEliminati { get; set; } = new();
    public HashSet<string> EventiCompletati { get; set; } = new();

    // public List<Quest> QuestAttive { get; set; } = new();
    // public List<Quest> QuestCompletate { get; set; } = new();
}