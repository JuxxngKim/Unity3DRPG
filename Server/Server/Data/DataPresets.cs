using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Data
{
    public static class DataPresets
    {
        public static StatInfo MakeGanyuStat(int level)
        {
            StatInfo stat = new StatInfo();
            stat.Level = level;
            stat.Attack = level * 10;
            stat.Hp = stat.MaxHp = 10 * level;
            stat.Speed = 7.0f;
            stat.Radius = 0.7f;
            return stat;
        }

        public static StatInfo MakeChuChuStat(int level)
        {
            StatInfo stat = new StatInfo();
            stat.Level = level;
            stat.Attack = level * 2;
            stat.Hp = stat.MaxHp = 5 * level;
            stat.Speed = 5.0f;
            stat.Radius = 1.0f;
            return stat;
        }

        public static SkillData BasicProjectile
        {
            get
            {
                SkillData skillData = new SkillData()
                {
                    Id = 1,
                    Type = SkillType.Projectile,
                    Damage = 10,
                    CoolTimeFrame = 10,
                    StateFrame = 10,
                    MoveSpeed = 20,
                    Range = 0.7f,
                };

                return skillData;
            }
        }

        public static SkillData Meteo
        {
            get
            {
                SkillData skillData = new SkillData()
                {
                    Id = 2,
                    Type = SkillType.Area,
                    Damage = 30,
                    CoolTimeFrame = 10,
                    StateFrame = 15,
                    MoveSpeed = 0,
                    Range = 4.0f,
                };

                return skillData;
            }
        }
    }

    public class SkillData
    {
        public int Id;
        public SkillType Type;
        public int Damage;
        public int CoolTimeFrame;
        public int StateFrame;
        public int MoveSpeed;
        public float Range;
    }
}
