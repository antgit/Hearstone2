﻿using System.Linq;
using Hearstone2.Core.Cards;
using Hearstone2.Core.Heroes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hearthtone2.MonoFront.Components
{
	public class PlacedMinionsComponent : BaseCardGameComponent
	{
		public PlacedMinionsComponent(Hearthtone2Game game, Hero owner, Point position)
			: base(game, owner, new Rectangle(position.X, position.Y, game.GraphicsDevice.Viewport.Width, PlacedCard.Height))
		{
		}

        public override void Update(GameTime gameTime)
		{
			InitCards(Owner.PlacedMinions.Select((card, index) =>new PlacedCard{Card = card,Position = new Rectangle(index*200, Position.Y, PlacedCard.Width, PlacedCard.Height),Color = Color.White}).ToList());

			base.Update(gameTime);
		}

		public override void OnCardOver(PlacedCard card)
		{
			var minion = card.Card as Minion;

 			switch (Game.CurrentGameMode)
	        {
		        case GameMode.SelectCard:
					if (Game.Table.CurrentPlayer == Owner && Owner.IsAlive)
					{
                        card.Color = ((Minion)card.Card).CanFight && ((Minion)card.Card).Damage > 0 ? Color.LightGreen : Color.Red;
					}
			        break;
		        case GameMode.SelectTarget:
					if (Owner.CanMinionBeATargetOfAttack(minion) || Game.CurrentlyPlayingCard.Card is IMinionTargetSpell && ((IMinionTargetSpell)Game.CurrentlyPlayingCard.Card).CanPlay(minion))
			        {
				        card.Color = Color.LightGreen;
			        }
			        else
			        {
				        card.Color = Color.Red;
						var taunts = PlacedCards.Where(c => ((Minion)c.Card).IsTaunt).ToList();
						taunts.ForEach(t => t.Color = Color.LightGreen);
			        }
			        break;
	        }
		}

		public override void OnCardClick(PlacedCard card)
		{
			var targetMinion = card.Card as Minion;

 			switch (Game.CurrentGameMode)
            {
                case GameMode.SelectCard:
					if (targetMinion.CanFight && targetMinion.Damage > 0 && Game.Table.CurrentPlayer == Owner && Owner.IsAlive)
                    {
						Game.SelectTargetFor(card);
                    }

                    break;
                case GameMode.SelectTarget:
					if (Game.CurrentlyPlayingCard.Card is IMinionTargetSpell)
                    {
						((IMinionTargetSpell)Game.CurrentlyPlayingCard.Card).Play(targetMinion);
						Game.ResetGameMode();
					}
					else if (Game.CurrentlyPlayingCard.Card is Minion)
                    {
	                    if (Owner.CanMinionBeATargetOfAttack(targetMinion))
	                    {
							((Minion)Game.CurrentlyPlayingCard.Card).DealDamage(card.Card as Minion);
							((Minion)card.Card).DealDamage(Game.CurrentlyPlayingCard.Card as Minion);
							Game.ResetGameMode();
	                    }
	                    else
	                    {
		                    //Player should select taunt
	                    }
                    }

					Game.Table.Cleanup();
                    break;
            }
		}

		public override void Draw(GameTime gameTime)
		{
			var spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin();
            foreach (var card in PlacedCards)
            {
                Game.CardDrawer.Draw(card, spriteBatch);
            }
            spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
