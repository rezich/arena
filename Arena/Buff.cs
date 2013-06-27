using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Arena {
	public enum BuffAlignment {
		Positive,
		Negative
	}
	public enum BuffType {
		MoveSpeed,
		TurnSpeed,
		AttackSpeed,
		Damage,
		HealthRegen,
		EnergyRegen,
		BaseAttackTime,
		AttackRange,
		MaxHealth,
		MaxEnergy
	}
	public class Buff {
		public string Name;
		public BuffAlignment Type;
		public bool Hidden;
		public bool Permanent;
		public List<Tuple<BuffType, double>> Buffs = new List<Tuple<BuffType, double>>();
		public TimeSpan ExpirationTime;
		public Buff(string name, BuffAlignment type, BuffType buffType, double value, TimeSpan? expirationTime, bool hidden) {
			Name = name;
			Type = type;
			Buffs = new List<Tuple<BuffType, double>>() { new Tuple<BuffType, double>(buffType, value) };
			if (expirationTime.HasValue)
				ExpirationTime = (TimeSpan)expirationTime;
			else
				Permanent = true;
			Hidden = hidden;
		}
		public Buff(string name, BuffAlignment type, List<Tuple<BuffType, double>> buffs, TimeSpan? expirationTime, bool hidden) {
			Name = name;
			Type = type;
			Buffs = buffs;
			if (expirationTime.HasValue)
				ExpirationTime = (TimeSpan)expirationTime;
			else
				Permanent = true;
			Hidden = hidden;
		}
		public double ValueOf(BuffType type) {
			double returnValue = 0;
			foreach (Tuple<BuffType, double> t in Buffs) {
				if (t.Item1 == type)
					returnValue += t.Item2;
			}
			return returnValue;
		}
	}
}