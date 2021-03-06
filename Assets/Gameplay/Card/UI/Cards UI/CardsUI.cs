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
	public class CardsUI : MonoBehaviour
	{
		[SerializeField]
        protected CardsUICreator _UICreator;
        public CardsUICreator UICreator { get { return _UICreator; } }
        public virtual void CreateUI()
        {
            UICreator.DestroyInstances();

            UICreator.Create(Level.CardsManager.Inventory.List);

            for (int i = 0; i < UICreator.Instances.Count; i++)
                InitCardTemplate(UICreator.Instances[i]);

            if(QueryPredicate != null)
                Query(QueryPredicate);
        }
        protected virtual void InitCardTemplate(CardUITemplate template)
        {
            template.OnClick += () => OnCardClick(template);
        }

        public event Action<CardUITemplate> OnClick;
        protected virtual void OnCardClick(CardUITemplate template)
        {
            if (OnClick != null)
                OnClick(template);
        }

        public Level Level { get { return References.Level; } }

        protected virtual void Start()
        {
            Level.CardsManager.Inventory.OnAdd += OnCardsChanged;
            Level.CardsManager.Inventory.OnUse += OnCardsChanged;

            CreateUI();
        }

        protected virtual void OnEnable()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(UICreator.Parent);
        }

        public Predicate<Card> QueryPredicate { get; protected set; }
        public virtual void Query(Predicate<Card> predicate)
        {
            QueryPredicate = predicate;

            for (int i = 0; i < UICreator.Instances.Count; i++)
                UICreator.Instances[i].gameObject.SetActive(predicate(UICreator.Instances[i].Data));
        }
        public virtual void ClearQuery()
        {
            QueryPredicate = null;

            for (int i = 0; i < UICreator.Instances.Count; i++)
            {
                UICreator.Instances[i].gameObject.SetActive(true);
            }
        }

        protected virtual void OnCardsChanged(Card card)
        {
            CreateUI();
        }
    }

    [Serializable]
    public class CardsUICreator : ListUICreator<Card, CardUITemplate>
    {
        public override void Create(IList<Card> data)
        {
            base.Create(data);

            LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
        }
    }
}