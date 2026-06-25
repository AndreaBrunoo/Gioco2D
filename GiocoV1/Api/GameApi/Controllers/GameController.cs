using Microsoft.AspNetCore.Mvc;
using GiocoV1.Modelli;

namespace GameApi.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private static readonly Lazy<StatoGioco> _gameState = new Lazy<StatoGioco>(InitializeGameState);

    private static StatoGioco InitializeGameState()
    {
        return new StatoGioco
        {
            Personaggio = new Personaggio
            {
                Nome = "Eroe",
                SaluteMassima = 100,
                SaluteAttuale = 100,
                Attacco = 10,
                Difesa = 5,
                Velocita = 7,
                Livello = 1,
                Esperienza = 0,
                Monete = 50,
                PuntiAbilita = 0,
                Mosse = new List<Mossa>(),
                Inventario = new List<OggettoInventario>(),
                Equipaggiamenti = new Equipaggiamento()
            },
            AreaCorrente = "Pianura Iniziale",
            PosizioneX = 0,
            PosizioneY = 0,
            BossSconfitti = new HashSet<string>(),
            OggettiRaccolti = new HashSet<string>(),
            NemiciEliminati = new HashSet<string>(),
            EventiCompletati = new HashSet<string>()
        };
    }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        return Ok(_gameState.Value);
    }

    [HttpPost("action")]
    public IActionResult PerformAction([FromBody] string action)
    {
        var state = _gameState.Value;

        switch (action.ToLower())
        {
            case "moveup":
                state.PosizioneY--;
                break;
            case "movedown":
                state.PosizioneY++;
                break;
            case "moveleft":
                state.PosizioneX--;
                break;
            case "moveright":
                state.PosizioneX++;
                break;
            default:
                return BadRequest("Azione sconosciuta");
        }

        // Update timestamp
        state.Timestamp = DateTime.Now;

        return Ok(state);
    }
}
