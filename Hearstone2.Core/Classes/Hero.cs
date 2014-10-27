﻿using System.Collections.Generic;
using System.Linq;
using Hearstone2.Core.Cards;

namespace Hearstone2.Core.Classes
{
	public abstract class Hero
	{
		public List<Card> Hand { get; private set; }
		public List<Card> Deck { get; set; }
		public List<Minion> PlacedMinions { get; private set; }
        public Hero Opponent { get; set; }
        public int Mana { get; set; }
		public int MaxMana { get; set; }
        public int Health { get; set; }

		public Hero()
		{
			Hand = new List<Card>();
			Deck = new List<Card>();
			PlacedMinions = new List<Minion>();
			MaxMana = 0;
		    Health = 30;
		}

		public abstract void HeroAbility();

		public void GainMana()
		{
			MaxMana++;
			Mana = MaxMana;
		}

		public void DrawCard()
		{
			var card = Deck.FirstOrDefault();

			if (card == null)
			{
				//TODO: Deal damage to player
				return;
			}

			card.Owner = this;
			Hand.Add(card);
			Deck.Remove(card);
		}

        public void RefreshMinions()
        {
            PlacedMinions.ForEach(m => m.Refresh());
        }

        public void ReceiveDamage(int damage)
        {
            Health -= damage;
        }
	}
}