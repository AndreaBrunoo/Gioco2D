using Microsoft.AspNetCore.Mvc;
using GiocoV1.Modelli;
using System;

namespace GameApi.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private static StatoGioco? _currentState;

    [HttpGet("state")]
    public IActionResult GetState()
    {
        if (_currentState == null)
        {
            // Initialize a new game state
            _currentState = new StatoGioco
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
                    PuntiAbilita = 0
                },
                AreaCorrente = "Pianura Iniziale",
                PosizioneX = 0,
                PosizioneY = 0
            };
        }
        return Ok(_currentState);
    }

    [HttpPost("action")]
    public IActionResult PerformAction([FromBody] string action)
    {
        if (_currentState == null)
        {
            return BadRequest("Game not initialized");
        }

        // Very simple action handling for demonstration
        switch (action.ToLower())
        {
            case "moveup":
                _currentState.PosizioneY--;
                break;
            case "movedown":
                _currentState.PosizioneY++;
                break;
            case "moveleft":
                _currentState.PosizioneX--;
                break;
            case "moveright":
                _currentState.PosizioneX++;
                break;
            default:
                return BadRequest("Unknown action");
        }

        // Update timestamp
        _currentState.Timestamp = DateTime.Now;

        return Ok(_currentState);
    }
}
