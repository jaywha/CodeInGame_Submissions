using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    private static UnitModel Queen;
    private static UnitModel EnemyQueen;
    private static Dictionary<int, SiteModel> Sites;
    private static Dictionary<int, UnitModel> Units;
    const int MAX_X = 1920;
    const int MAX_Y = 1000;

    static void Main(string[] args)
    {
        SiteModel kBarracks = null;
        SiteModel aBarracks = null;

        string[] inputs;
        int numSites = int.Parse(Console.ReadLine());
        Sites = new();
        Units = new();
        for (int i = 0; i < numSites; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            Sites[i] = new SiteModel(inputs);
        }

        Console.Error.WriteLine($"Site Count: {Sites.Count}");

        // game loop
        while (true)
        {
            int knights = 0;
            int archers = 0;
            Units.Clear();

            inputs = Console.ReadLine().Split(' ');
            int gold = int.Parse(inputs[0]);
            int touchedSite = int.Parse(inputs[1]); // -1 if none
            for (int i = 0; i < numSites; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                Sites[i] = Sites[i].UpdateSite(inputs);
            }
            int numUnits = int.Parse(Console.ReadLine());
            for (int i = 0; i < numUnits; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                if (int.Parse(inputs[3]) == -1) {
                    if (int.Parse(inputs[2]) == 0) {
                        if (Queen == null) {
                            Queen = new UnitModel().UpdateUnit(inputs);
                        } else {
                            Queen.UpdateUnit(inputs);
                        }
                    } else {
                        if (EnemyQueen == null) {
                            EnemyQueen = new UnitModel().UpdateUnit(inputs);
                        } else {
                            EnemyQueen.UpdateUnit(inputs);
                        }
                    }
                } else {
                    if (!Units.ContainsKey(i)) {
                        Units[i] = new UnitModel().UpdateUnit(inputs);
                    } else {
                        Units[i] = Units[i].UpdateUnit(inputs);
                    }
                }
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");


            // First line: A valid queen action
            if (kBarracks?.underConstruction ?? false) {
                kBarracks.structureType = 2;
                kBarracks.underConstruction = false;
            }

            if (aBarracks?.underConstruction ?? false) {
                aBarracks.structureType = 2;
                aBarracks.underConstruction = false;
            }

            if (touchedSite >= 0 && Sites[touchedSite].structureType == -1)
            {
                if (kBarracks == null)
                {
                    kBarracks = Sites[touchedSite];
                    kBarracks.underConstruction = true;
                    Console.WriteLine($"BUILD {touchedSite} BARRACKS-KNIGHT");
                }
                else if (aBarracks == null)
                {
                    aBarracks = Sites[touchedSite];
                    aBarracks.underConstruction = true;
                    Console.WriteLine($"BUILD {touchedSite} BARRACKS-ARCHER");
                } else {
                    DoQueenThings();
                }
            }
            else
            {
                DoQueenThings();
            }

            // Second line: A set of training instructions
            if (gold >= 100 && archers < 2 && aBarracks?.structureType == 2)
            {
                Console.WriteLine($"TRAIN {aBarracks.siteId}");
            }
            else if (gold >= 80 && knights < 5 && kBarracks?.structureType == 2)
            {
                Console.WriteLine($"TRAIN {kBarracks.siteId}");
            }
            else
            {
                Console.WriteLine("TRAIN");
            }
        }

        static void DoQueenThings() {
            var (x, y) = RetreatFromClosestEnemy();
            if (x == 0 && y == 0) {
                (x, y) = GoToClosestSite();
            }
            Console.WriteLine($"MOVE {x} {y}");
        }

        static (int, int) GoToClosestSite() {
            if (Queen == null || Units == null || Units.Count == 0) {
                return (0,0);
            }

            var closestSite = FindClosestSite();
            var firstSite = Sites.FirstOrDefault().Value;
            var firstSite_x = firstSite.position.x - firstSite.radius;
            var firstSite_y = firstSite.position.y - firstSite.radius;
            int modX = clamp(0, closestSite?.position.x ?? firstSite_x);
            int modY = clamp(1, closestSite?.position.y ?? firstSite_y);

            return (modX, modY);
        }

        static SiteModel FindClosestSite() {
            if (Sites == null || Sites.Count == 0) {
                return null;
            }

            SiteModel closestSite = null;
            double minDistance = 0.0;
            var sites = Sites.Where(kvp => kvp.Value.structureType == -1).ToList();
            if (sites.Any()) {
                foreach(var site in sites) {
                    var distance = Queen.position.DistanceTo(site.Value.position);
                    if (distance < minDistance) {
                        minDistance = distance;
                        closestSite = site.Value;
                    }
                }
            }
            return closestSite;
        }

        static (int, int) RetreatFromClosestEnemy() {
            if (Queen == null || Units == null || Units.Count == 0) {
                return (0,0);
            }

            int modX = 0;
            int modY = 0;
            var closestEnemy = FindClosestEnemy();

            if (closestEnemy != null) {
                modX = clamp(0, closestEnemy.position.x);
                modY = clamp(1, closestEnemy.position.y);
            }

            return (modX, modY);
        }

        static UnitModel FindClosestEnemy() {
            UnitModel closestEnemy = null;
            double? minDistance = null;
            var enemies = Units.Where(kvp => kvp.Value.owner == 1 && kvp.Value.unitType != -1).ToList();
            if (enemies.Any()) {
                foreach(var enemy in enemies) {
                    var distance = Queen.position.DistanceTo(enemy.Value.position);
                    if (minDistance.HasValue && distance < minDistance.Value) {
                        minDistance = distance;
                        closestEnemy = enemy.Value;
                    }
                }
            }
            return closestEnemy;
        }

        static int clamp(int axis, int position) {
            if (axis == 0) {
                var max = position >= MAX_X - 100 ? -40 : 20;
                var min = position <= 100 ? 40 : -20;

                return position + (max > position ? max : -min);
            } else if (axis == 1) {
                var max = position >= MAX_Y - 80 ? -30 : 15;
                var min = position <= 80 ? 30 : -15;

                return position + (max > position ? max : -min);
            } else {
                return 0;
            }
        }
    }

    public class SiteModel {
        public int siteId;
        public Point position;
        public int radius;
        public int ignore1;
        public int ignore2;
        public int structureType;
        public int owner;
        public int param1;
        public int param2;
        public bool underConstruction = false;

        public SiteModel(string[] inputs) {
            siteId = int.Parse(inputs[0]);
            position = new Point(int.Parse(inputs[1]), int.Parse(inputs[2]));
            radius = int.Parse(inputs[3]);
        }

        public SiteModel UpdateSite(string[] inputs) {
            // Console.Error.WriteLine($"Parsing site data: {inputs.Aggregate((c,n)=>c+", "+n)}");            
            siteId = int.Parse(inputs[0]);
            ignore1 = int.Parse(inputs[1]); // used in future leagues
            ignore2 = int.Parse(inputs[2]); // used in future leagues
            structureType = int.Parse(inputs[3]); // -1 = No structure, 2 = Barracks
            owner = int.Parse(inputs[4]); // -1 = No structure, 0 = Friendly, 1 = Enemy
            param1 = int.Parse(inputs[5]);
            param2 = int.Parse(inputs[6]);
            return this;
        }
    }

    public class UnitModel {
        public Point position;
        public int owner;
        public int unitType;
        public int health;

        public UnitModel UpdateUnit(string[] inputs) {
            if (this.position == null) {
                this.position = new Point(int.Parse(inputs[0]), int.Parse(inputs[1]));
            } else {
                this.position.x = int.Parse(inputs[0]);
                this.position.y = int.Parse(inputs[1]);
            }
            this.owner = int.Parse(inputs[2]);
            this.unitType = int.Parse(inputs[3]); // -1 = QUEEN, 0 = KNIGHT, 1 = ARCHER
            this.health = int.Parse(inputs[4]);
            return this;
        }
    }

    public class Point {
        public int x;
        public int y;

        public Point(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public double DistanceTo(Point other) {
            return Math.Sqrt(Math.Pow(this.x-other.x,2)+Math.Pow(this.y-other.y,2));
        }
    }
}