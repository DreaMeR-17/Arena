﻿using System;
using System.Collections.Generic;

namespace Arena
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Arena arena = new Arena();
            arena.Play();
        }
    }

    class Arena
    {
        private List<Fighter> _fighters;
        private List<Fighter> _fightingFighters;

        public Arena()
        {
            _fighters = new List<Fighter>()
            {
                new Gladiator(),
                new Valkyrie(),
                new Highlander(),
                new Shaman(),
                new Berserk(),
            };

            _fightingFighters = new List<Fighter>();
        }

        public void Play()
        {
            const char CommandShowInfoFighters = '1';
            const char CommandChooseFighters = '2';
            const char CommandExit = '0';

            bool isWork = true;

            while (isWork)
            {
                Console.Write("\nВыберите действие: \n" +
                CommandShowInfoFighters + ". Вывести информацию о бойцах\n" +
                CommandChooseFighters + ". Выбрать бойцов для поединка.\n" +
                CommandExit + ". Покинуть арену.\n" +
                "Ввод: ");

                char userInput = Console.ReadKey(true).KeyChar;

                Console.WriteLine("\n");

                Console.Clear();

                switch (userInput)
                {
                    case CommandShowInfoFighters:
                        ShowInfoFighters();
                        break;

                    case CommandChooseFighters:
                        ChooseFighter();
                        break;

                    case CommandExit:
                        isWork = false;
                        break;

                    default:
                        Console.WriteLine("Такой комнады нет. Повторите попытку.");
                        break;
                }
            }
        }

        private void CompletionFight()
        {
            int firstFighter = 0;
            int secondFighter = 1;

            Fighter fighter1 = _fightingFighters[firstFighter];
            Fighter fighter2 = _fightingFighters[secondFighter];
            Fight(fighter1, fighter2);

            Console.WriteLine("\nБой завершен.");

            ClearArena();
        }

        private void Fight(Fighter fighter, Fighter opponent)
        {
            string line = new string('#', 50);

            while (fighter.Health > 0 && opponent.Health > 0)
            {
                fighter.ShowInfo();
                fighter.Attack(opponent);

                Console.WriteLine("\n" + line);

                opponent.ShowInfo();

                if (opponent.Health > 0)
                    opponent.Attack(fighter);

                Console.WriteLine("\n" + line + "\n");
            }

            AnnounceWinner(fighter, opponent);
        }

        private void ChooseFighter()
        {
            Console.Write("Выберите первого бойца: ");
            Fighter firstFighter = GetFighter().Clone();
            _fightingFighters.Add(firstFighter);

            Console.Write("Выберите второго бойца: ");
            Fighter secondFighter = GetFighter().Clone();

            if (firstFighter.Name == secondFighter.Name)
                secondFighter = secondFighter.Clone(firstFighter.Name + ".1");
            else
                secondFighter = secondFighter.Clone();

            _fightingFighters.Add(secondFighter);

            Console.Write("\n");

            CompletionFight();
        }

        private void AnnounceWinner(Fighter fighter1, Fighter fighter2)
        {
            if (fighter1.Health <= 0)
                Console.WriteLine($"{fighter2.Name} одержал победу над {fighter1.Name}");
            else if (fighter2.Health <= 0)
                Console.WriteLine($"{fighter1.Name} одержал победу над {fighter2.Name}");
        }

        private Fighter GetFighter()
        {
            Fighter chosenFighter = null;

            while (chosenFighter == null)
            {
                int index = GetNumber(Console.ReadLine());

                if (index <= _fighters.Count && index > 0)
                    chosenFighter = _fighters[index - 1];
                else
                    Console.WriteLine("Неверный ввод номера бойца.");
            }

            return chosenFighter;
        }

        private void ShowInfoFighters()
        {
            foreach (Fighter fighter in _fighters)
                fighter.ShowInfo();
        }

        private void ClearArena()
        {
            _fightingFighters.Clear();
        }

        private int GetNumber(string originalString)
        {
            int number;

            while (int.TryParse(originalString, out number) == false)
            {
                Console.Write("Ошибка. Повторите попытку: ");
                originalString = Console.ReadLine();
            }

            return number;
        }
    }

    abstract class Fighter
    {
        public Fighter(string name, float health, float armor, float damage)
        {
            Name = name;
            Health = health;
            Armor = armor;
            AttackDamage = damage;
        }

        public string Name { get; private set; }
        public float Health { get; protected set; }
        public float Armor { get; protected set; }
        public float AttackDamage { get; protected set; }

        public virtual void ShowInfo()
        {
            Console.WriteLine($"{Name}:");
            Console.WriteLine($"Количество HP: {(int)Health}");
            Console.WriteLine($"Наносимый урон: {(int)AttackDamage}\n");
        }

        public virtual void TakeDamage(float damage)
        {
            if (Armor > 0 && Armor <= damage)
                Health -= damage - Armor;
            else if (Armor > damage)
                Health -= 1;
            else
                Health -= damage;
        }

        public virtual void Attack(Fighter opponent)
        {
            opponent.TakeDamage(AttackDamage);
            Console.WriteLine($"{Name} атакует {opponent.Name} и наносит у {AttackDamage} урона.");
        }

        public abstract Fighter Clone(string newName = null);

        protected void TryUseSkill(out bool skill)
        {
            int minChanceSkill = 1;
            int maxChanceSkill = 100;
            int skillChance = 50;

            skill = UserUtils.GenerateRandomNumber(minChanceSkill, maxChanceSkill) < skillChance;
        }
    }

    class Gladiator : Fighter
    {
        public Gladiator(string name = "Гладиатор") : base(name, 150f, 15f, 35f) { }

        public override void Attack(Fighter opponent)
        {
            TryUseSkill(out bool IsDoubleDamage);

            if (IsDoubleDamage)
            {
                int multiplier = 2;
                float attackDoubleDamage = AttackDamage * multiplier;

                opponent.TakeDamage(attackDoubleDamage);
                Console.WriteLine($"{Name} атакует {opponent.Name} и наносит удвоенный урон: {attackDoubleDamage}.");
            }
            else
            {
                base.Attack(opponent);
            }
        }

        public override Fighter Clone(string newName = null)
        {
            string name = newName ?? Name;

            return new Gladiator(name);
        }
    }

    class Valkyrie : Fighter
    {
        private int _countOfAttacks = 0;

        public Valkyrie(string name = "Валькирия") : base(name, 200f, 15, 35f) { }

        public override void Attack(Fighter opponent)
        {
            int everyThirdAttack = 3;
            int countOfAttacks = 2;
            int currentAttack = 1;

            if (_countOfAttacks % everyThirdAttack == 0 && _countOfAttacks > 0)
            {
                while(currentAttack <= countOfAttacks)
                {
                    opponent.TakeDamage(AttackDamage);
                    currentAttack++;
                }

                Console.WriteLine($"{Name} дважды атакует {opponent.Name} и наносит {AttackDamage}, а затем {AttackDamage} урона.");
                ++_countOfAttacks;
            }
            else
            {
                base.Attack(opponent);
                ++_countOfAttacks;
            }
        }

        public override Fighter Clone(string newName = null)
        {
            string name = newName ?? Name;

            return new Valkyrie(name);
        }
    }

    class Highlander : Fighter
    {
        private int _maxRage = 100;

        public Highlander(string name = "Горец") : base(name, 300, 0, 45f)
        {
            Rage = 0;
        }

        public int Rage { get; private set; }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            AccumulateRage();
        }

        public override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"\tЯрость: {Rage}\n");
        }

        public override Fighter Clone(string newName = null)
        {
            string name = newName ?? Name;

            return new Highlander(name);
        }

        private void AccumulateRage()
        {
            int percentageRage = 25;
            Rage += percentageRage;

            if (Rage == _maxRage)
                Heal();
        }

        private void Heal()
        {
            int percentageHealth = 20;
            Health += percentageHealth;
            Rage -= _maxRage;

            Console.WriteLine($"{Name}: Я чувствую прилив сил!");
        }
    }

    class Shaman : Fighter
    {
        public Shaman(string name = "Шаман") : base(name, 100f, 20f, 50f)
        {
            Mana = 0;
        }

        public int Mana { get; private set; }

        public float ThrowFireball(int percentageMana)
        {
            float multiplier = 1.5f;

            SpendMana(percentageMana);

            return AttackDamage * multiplier;
        }

        public override void Attack(Fighter opponent)
        {
            int percentageMana = 6;

            if (Mana >= percentageMana)
            {
                opponent.TakeDamage(ThrowFireball(percentageMana));
                Console.WriteLine($"{Name} использовал заклинание.");
            }
            else
            {
                base.Attack(opponent);
            }

            int manaIncrement = 2;
            Mana += manaIncrement;
        }

        public override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"\tМана: {Mana}\n");
        }

        public override Fighter Clone(string newName = null)
        {
            string name = newName ?? Name;

            return new Shaman(name);
        }

        private void SpendMana(int percentageMana) => Mana -= percentageMana;
    }

    class Berserk : Fighter
    {
        public Berserk(string name = "Берсерк") : base(name, 170f, 10f, 40f) { }

        public override void TakeDamage(float damage)
        {
            TryUseSkill(out bool IsEvasion);

            if (IsEvasion)
            {
                Console.WriteLine($"{Name}: Я успешно увернулся от удара.");

                return;
            }
            else
                base.TakeDamage(damage);
        }

        public override Fighter Clone(string newName = null)
        {
            string name = newName ?? Name;

            return new Berserk(name);
        }
    }

    class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int maxValue)
        {
            return s_random.Next(maxValue);
        }

        public static int GenerateRandomNumber(int minValue, int maxValue)
        {
            return s_random.Next(minValue, maxValue);
        }
    }
}