using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace MatchController
    {
    public class MatchController : MonoBehaviour
    {
        public TextMeshProUGUI informationMessage;

        private MatchTimeController _matchTimeController;
        private TeamController _teamController;
        private MapData _mapData;
        private Ball _b;
        private GUIStyle _style;
        private bool? _hasStarted = null;
        public GameObject ExplosionParticleSystem;

        void Awake()
        {
            _mapData = transform.Find("World").GetComponentInChildren<MapData>();
            _matchTimeController = GetComponent<MatchTimeController>();
            _teamController = GetComponent<TeamController>();
            _b = transform.Find("Ball").GetComponent<Ball>();

            // On s'assure qu'il est desactiv� par d�faut
            _matchTimeController.enabled = false;
            // D�but de partie 
            StartCoroutine(startTimer());


            /*
            _style = new GUIStyle
            {
                normal = {textColor = new Color(1f, 0.25f, 0.15f)},
                fontSize = 25,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperCenter
            };
            */
        }

        void Update()
        {
            if (_hasStarted == false)
            {
                _hasStarted = true;
                _matchTimeController.enabled = true;
                _teamController.Initialize();
            }

            /*
            if (_matchTimeController.MatchTimer < 1.0f && !_matchTimeController.Overtime)
            {
                if (_b != null)
                _matchTimeController.paused = !_b.isTouchedGround;
            }
            */

            if (_matchTimeController.HasEnded()) // && _mapData.blueScore == _mapData.orangeScore
            {
                // _matchTimeController.ActivateOvertime();
                // ResetGameState();
                manageEndGame();
            }
            /*
            if(_mapData.isScoredBlue || _mapData.isScoredOrange)
            {
                HandleScoreEvent();
            }
            */
        }

        IEnumerator startTimer()
        {
            int delay = 3;
            
 
            for (int i = delay; i >= 0; --i)
            {
                Debug.LogWarning("bonjour" + Time.time);
                char pluriel = i <= 1 ? '\0' : 's';
                informationMessage.text = $"ESIPE VS ESIEE\nLa partie va commencer dans {i} seconde{pluriel} !";
                yield return new WaitForSeconds(1f);
            }

            informationMessage.text = "";
            _hasStarted = false;
        }

        private void manageEndGame()
        {
            if (!_matchTimeController || _matchTimeController.enabled == false)
            {
                return;
            }

            if(_mapData.blueScore == _mapData.orangeScore)
            {
                informationMessage.text = "Match nul !";
            }
            else if(_mapData.blueScore > _mapData.orangeScore)
            {
                informationMessage.text = "L'�quipe ESIPE a gagn�e !";
            }
            else if (_mapData.orangeScore > _mapData.blueScore)
            {
                informationMessage.text = "L'�quipe ESIEE a gagn�e !";
            }
        }

        private void ResetGameState()
        {
            _matchTimeController.paused = true;
            _teamController.SpawnTeams();
            _b.ResetBall();
            _mapData.ResetIsScored();
            _matchTimeController.paused = false;
        }

        /*
        private void HandleScoreEvent()
        {
            ResetGameState();
            if (_matchTimeController.Overtime)
            {
                _matchTimeController.EndOvertime();
            }
        }
        */

        /*
        private void OnGUI()
        {
            if (_matchTimeController.HasEnded())
            {
                GUI.Label(new Rect(Screen.width / 2 - 75, Screen.height / 2 - 75, 150, 130), "GAME OVER", _style);
            }
        }
        */

        public void HandleDemolition(GameObject demolishedCar)
        {
            TeamController.Team team = _teamController.GetTeamOfCar(demolishedCar);
            StartCoroutine(CarRespawn(demolishedCar, team));
        }

        IEnumerator CarRespawn(GameObject demolishedCar, TeamController.Team team)
        {
            GameObject explosion = Instantiate(ExplosionParticleSystem);
            explosion.transform.position = demolishedCar.transform.position;
            explosion.GetComponent<ParticleSystem>().Play();
            Destroy(explosion, 2f);
            demolishedCar.transform.localPosition = new Vector3(0f, -5f, 0f);
            demolishedCar.GetComponent<InputManager>().enabled = false;
            yield return new WaitForSeconds(3f);
            GetComponent<SpawnController>().SpawnCar(demolishedCar, team, true);
            demolishedCar.GetComponent<InputManager>().enabled = true;
        }

    }

}
