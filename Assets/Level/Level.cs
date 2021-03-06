﻿using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using Moe.Tools;

namespace Game
{
	public class Level : MonoBehaviour
	{
        public static Level Current { get; protected set; }
        protected virtual void SetCurrent()
        {
            Current = this;
        }

        [SerializeField]
        protected Castle castle;
        public Castle Castle { get { return castle; } }
        public bool IsPlaying { get { return castle.Health != 0f; } }

        [SerializeField]
        protected WaveSystem waveSystem;
        public WaveSystem WaveSystem { get { return waveSystem; } }

        [SerializeField]
        protected ScoreManager scoreManager;
        public ScoreManager ScoreManager { get { return scoreManager; } }

        [SerializeField]
        protected CardsManager cardsManager;
        public CardsManager CardsManager { get { return cardsManager; } }

        [SerializeField]
        protected CardsPlotManager cardsPlotManager;
        public CardsPlotManager CardsPlotManager { get { return cardsPlotManager; } }

        [SerializeField]
        protected TowersManager towersManager;
        public TowersManager TowersManager { get { return towersManager; } }

        [SerializeField]
        protected EnemiesManager enemiesManager;
        public EnemiesManager EnemiesManager { get { return enemiesManager; } }

        public SFXManager SFXManager;

        public DeathMenu DeathMenu;

        public LevelPause Pause { get; protected set; }

        public InGameMenu Menu { get; protected set; }

        public class Module : MoeLinkedBehaviourModule<Level>
        {
            public Level Level { get { return Link; } }
        }


        protected virtual void Awake()
        {
            SetCurrent();
        }


        protected virtual void Start()
        {
            InitPause();

            InitMenu();

            waveSystem.Init();

            castle.OnDied += OnDeath;
        }

        private void OnDeath(IDamager obj)
        {
            waveSystem.StopAllCoroutines();

            DeathMenu.Show();
        }

        protected virtual void InitPause()
        {
            Pause = GetComponent<LevelPause>();

            if (Pause == null)
                throw MoeTools.ExceptionTools.Templates.MissingDependacny<LevelPause, Level>(name);

            Pause.Init(this);

            Pause.OnChanged += OnPauseStateChanged;
        }

        protected virtual void InitMenu()
        {
            Menu = FindObjectOfType<InGameMenu>();

            if (Menu == null)
                throw new Exception("No InGame Menu found in current scene");

            Menu.Init();
        }


        protected virtual void OnPauseStateChanged(LevelPauseState pauseState)
        {
            Menu.PauseMenu.UpdateState();
        }
    }

    public static partial class References
    {
        public static Level Level { get { return Level.Current; } }
    }
}