using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public struct Player {
    public string name;
    public int score;
}

public class GameManagerScript : NetworkBehaviour {
    
    public Text ScoreTextPrefab;

    private int playerId = 0;
    
    public readonly SyncDictionary<int, Player> PlayerList = new SyncDictionary<int, Player>();

    //Server only?
    public override void OnStartClient() {
        // Equipment is already populated with anything the server set up
        // but we can subscribe to the callback in case it is updated later on
        int lastKey = PlayerList.OrderBy(x => x.Key).Last().Key;
        PlayerList.Callback += OnPlayerListChange;
    }

    private void OnPlayerListChange(SyncDictionary<int, Player>.Operation op, int key, Player player) {
        // equipment changed,  perhaps update the gameobject
        Debug.Log(op + " - " + key + " - " + player);
    }
    
    //Send new player info to server (to add to dict)
    private void CmdPlayerConnect(string name) {
        PlayerList.Add(playerId, new Player { name = name, score = 0 });
        playerId++;
    }
    
    private void CmdPlayerDisconnect(int id) {
        if(PlayerList.ContainsKey(id)){
            PlayerList.Remove(id);
        }
    }
    
}
