using System;

namespace RPGProject.Utils
{
    public static class RandomUtils
    {
        private static readonly Random _rng = new Random();

        /// <summary>
        /// Retorna um número inteiro aleatório entre min (inclusivo) e max (exclusivo).
        /// </summary>
        public static int Range(int min, int max)
        {
            return _rng.Next(min, max);
        }

        /// <summary>
        /// Simula uma rolagem de dado (por exemplo, D6 = 6 lados).
        /// </summary>
        public static int Roll(int sides)
        {
            return _rng.Next(1, sides + 1);
        }

        /// <summary>
        /// Retorna verdadeiro com base em uma chance percentual (0 a 100).
        /// </summary>
        public static bool Chance(int percent)
        {
            return _rng.Next(0, 100) < percent;
        }

        /// <summary>
        /// Retorna um valor crítico (por exemplo, chance de acerto crítico).
        /// </summary>
        public static bool CriticalHit(int chancePercent = 10)
        {
            return Chance(chancePercent);
        }
    }
}
