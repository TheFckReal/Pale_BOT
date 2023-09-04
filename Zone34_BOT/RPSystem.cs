using Amazon.Runtime.Internal;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NLog;
using System.Text;
using static Pale_BOT.RPSystem.User.Person;
using static Pale_BOT.RPSystem.User;
using static Pale_BOT.RPSystem.User.Person.Skills;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Pale_BOT
{
    public static class RPGlobals
    {
        public const int StandartFreePoints = 18;
    }
    enum PerksId : long
    {
        Intelligence = 1,
        Mentality,
        Physic,
        Motility,
        All
    }

    [Serializable]
    public class CreatingCharacterException : Exception
    {
        public CreatingCharacterException() { }
        public CreatingCharacterException(string message) : base(message) { }
        public CreatingCharacterException(string message, Exception inner) : base(message, inner) { }
        protected CreatingCharacterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class ChangingCharacterException : Exception
    {
        public ChangingCharacterException() { }
        public ChangingCharacterException(string message) : base(message) { }
        public ChangingCharacterException(string message, Exception inner) : base(message, inner) { }
        protected ChangingCharacterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    internal class RPSystem
    {
        readonly ILogger _logger;
        internal RPSystem()
        {
            //_logger = LogManager.GetCurrentClassLogger();
        }

        public class User
        {
            [BsonId]
            public ulong UserId { get; set; }
            public List<Person> Persons { get; set; } = new List<Person>();
            public class Person
            {
                public string Name { get; set; } = "";
                public int Mental { get; set; }
                public int Health { get; set; }
                public int FreePoints { get; set; } = RPGlobals.StandartFreePoints;
                public Skills SkillSet { get; set; } = new Skills();
                public class Skills
                {
                    #region Perks Propeties
                    public IntellectSkill Intellect { get; set; } = new IntellectSkill();
                    public PsycheSkill Psyche { get; set; } = new PsycheSkill();
                    public PhysiqueSkill Physique { get; set; } = new PhysiqueSkill();
                    public MotoricsSkill Motorics { get; set; } = new MotoricsSkill();
                    #endregion
                    public interface ISkill
                    {
                        public string TranslatedName { get; }
                        public static Dictionary<string, string> StatDescription { get; } = null!;
                        public Dictionary<string, string> GetStatDescription() => StatDescription;
                        public List<Perk> PerkList { get; }
                        public int MaxPoints { get; set; }
                    }
                    public abstract record Perk
                    {
                        public int points = 1;
                        public bool isSpeciality;
                        public virtual string GetTranslatedName() => "Base perk name";
                    }
                    public class IntellectSkill : ISkill
                    {
                        #region non-serilizable-intelligence
                        [BsonIgnore]
                        public static Dictionary<string, string> StatDescription { get; } = new Dictionary<string, string>()
            {
                { "Логика", "Позволяет смотреть на вещи с неожиданной точки зрения, полагаясь на неожиданные ассоциации. Ваше “чувство прекрасного”, позволяющее находить нестандартные решения и оценивать произведения искусства" },
                { "Энциклопедия", "Это ваша общая эрудиция. Позволит припомнить исторические факты, найти в своей голове нужные знания и блеснуть ими в споре." },
                { "Риторика", "Риторика позволяет вести спор, искать бреши в аргументах оппонента и отстаивать свою позицию, полагаясь на факты." },
                { "Драма", "Помогает вам врать и понимать, когда обмануть пытаются вас. Также отвечает за притворство разного рода." },
                { "Концептуализация", "Позволяет смотреть на вещи с неожиданной точки зрения, полагаясь на неожиданные ассоциации. Также отвечает за ваше “чувство прекрасного”, позволяет находить нестандартные решения и оценивать произведения искусства." },
                { "Визуальный анализ", "Отражает ваши способности к математическому расчету, знания физики и т.п. Вы можете проследить траекторию полета пули, оценить расстояние от точки до точки, оценить глубину следов и многое другое." }
            };
                        public Dictionary<string, string> GetStatDescription() => StatDescription;
                        [BsonIgnore]
                        public string TranslatedName => "Интеллект";
                        [BsonIgnore]
                        public List<Perk> PerkList => new List<Perk> { Logic, Encyclopedia, Rhetoric, Drama, Сonceptualization, VisualCalculus };
                        #endregion
                        public LogicType Logic { get; set; } = new();
                        public EncyclopediaType Encyclopedia { get; set; } = new();
                        public RhetoricType Rhetoric { get; set; } = new();
                        public DramaType Drama { get; set; } = new();
                        public ConceptualizationType Сonceptualization { get; set; } = new();
                        public VisualCalculusType VisualCalculus { get; set; } = new();
                        #region intelligence-records
                        public record LogicType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Логика";
                            }
                        }
                        public record EncyclopediaType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Энциклопедия";
                            }
                        }
                        public record RhetoricType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Риторика";
                            }
                        }
                        public record DramaType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Драма";
                            }
                        }
                        public record ConceptualizationType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Концептуализация";
                            }
                        }
                        public record VisualCalculusType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Визуальный анализ";
                            }
                        }
                        #endregion
                        public int MaxPoints { get; set; } = 6;
                    }
                    public class PsycheSkill : ISkill
                    {
                        #region non-serilizable-mentality
                        [BsonIgnore]
                        public static Dictionary<string, string> StatDescription { get; } = new Dictionary<string, string>()
            {
                { "Сила воли", "Ваш моральный компас и защитник. Поможет вам сопротивляться манипуляциям, спасет от ментальных потрясений. Определяет сколько у вас очков Морали. Мораль упадет до 0 - будет нервный срыв." },
                { "Внутренняя империя", "Ваше воображение и не только. Отвечает за понимание паранормального. Является вашим “шестым чувством”." },
                { "Эмпатия", "Отражает то, насколько хорошо вы понимаете чувство окружающих. Вы можете заметить изменения в поведении, понять что чувствует другой человек и разделить это с ним." },
                { "Авторитет", "Ваше умение поставить себя в диалоге. Займите главенствующую позицию. Пусть вам подчиняются. Ваше слово - закон. Вы - закон." },
                { "Полицейская волна", "Позволяет лучше понимать ваших товарищей полицейских. Умение “быть на одной волне” с другими копами. Заодно ваша энциклопедия в мире полицейских и знание техник. Оказывает аналогичный эффект на другую профессию, если вы не полицейский." },
                { "Внушение", "Умение забраться в голову к другим людям и заставить их делать то, что вы хотите. Главное отличие от Риторики или Драмы в том, что вы заставляете людей думать, что они сами делают выбор." },
            };
                        public Dictionary<string, string> GetStatDescription() => StatDescription;
                        [BsonIgnore]
                        public string TranslatedName => "Психика";
                        [BsonIgnore]
                        public List<Perk> PerkList => new List<Perk> { Willpower, InlandEmpire, Empathy, Authority, PoliceWave, Suggestion };
                        #endregion
                        public WillpowerType Willpower { get; set; } = new();
                        public InlandEmpireType InlandEmpire { get; set; } = new();
                        public EmpathyType Empathy { get; set; } = new();
                        public AuthorityType Authority { get; set; } = new();
                        public PoliceWaveType PoliceWave { get; set; } = new();
                        public SuggestionType Suggestion { get; set; } = new();
                        #region mentality-records
                        public record WillpowerType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Сила воли";
                            }
                        }
                        public record InlandEmpireType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Внутренняя империя";
                            }
                        }
                        public record EmpathyType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Эмпатия";
                            }
                        }
                        public record AuthorityType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Авторитет";
                            }
                        }
                        public record PoliceWaveType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Полицейская волна";
                            }
                        }
                        public record SuggestionType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Внушение";
                            }
                        }
                        #endregion
                        public int MaxPoints { get; set; } = 6;
                    }
                    public class PhysiqueSkill : ISkill
                    {
                        #region non-serilizable-physic
                        [BsonIgnore]
                        public static Dictionary<string, string> StatDescription { get; } = new Dictionary<string, string>()
            {
                {"Стойкость", "Ваше физическое здоровье. Позволяет сопротивляться болезням, отравлениям и т.п. Определяет ваше здоровье, которое наряду с моралью является одним из двух основных ресурсов. Если кончится запас ХП, то вы умрете."},
                {"Болевой порог", "Позволяет вам не просто пережить ранения, а полностью их игнорировать. Включает и физическую, и психологическую боль."},
                {"Грубая сила", "То, насколько вы сильны. Ваш мышечный рельеф, прочность костей и т.п. Хотите дать кому-то в морду? Вам понадобится грубая сила."},
                {"Электрохимия", "Защищает вас от негативного воздействия лекарств, дает знания о наркотиках, алкоголе и т.п. Позволяет найти общий язык с теми, у кого есть зависимости."},
                {"Трепет", "Город говорит с вами. Позволяет ощущать малейшие изменения на улицах. Еще один вариант “шестого чувства”, но отвечает за чувствительность к бытовым вещам, а не мистике. Например, позволит почувствовать, куда идти в поисках улик"},
                {"Сумрак", "Бей или беги. Адреналин стучит в ушах, пока вы стучите кому-нибудь по роже. Позволяет запугивать людей, сопротивляться страху и преодолевать возможности своего организма."},
            };
                        public Dictionary<string, string> GetStatDescription() => StatDescription;
                        [BsonIgnore]
                        public string TranslatedName => "Физика";
                        [BsonIgnore]
                        public List<Perk> PerkList => new List<Perk> { Endurance, PainThreshold, PhysicalInstrument, ElectroChemistry, Shivers, HalfLight };
                        #endregion
                        public EnduranceType Endurance { get; set; } = new();
                        public PainThresholdType PainThreshold { get; set; } = new();
                        public PhysicalInstrumentType PhysicalInstrument { get; set; } = new();
                        public ElectroChemistryType ElectroChemistry { get; set; } = new();
                        public ShiversType Shivers { get; set; } = new();
                        public HalfLightType HalfLight { get; set; } = new();
                        #region physique-records
                        public record EnduranceType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Стойкость";
                            }
                        }
                        public record PainThresholdType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Болевой порог";
                            }
                        }
                        public record PhysicalInstrumentType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Грубая сила";
                            }
                        }
                        public record ElectroChemistryType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Электрохимия";
                            }
                        }
                        public record ShiversType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Трепет";
                            }
                        }
                        public record HalfLightType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Сумрак";
                            }
                        }
                        #endregion
                        public int MaxPoints { get; set; } = 6;
                    }
                    public class MotoricsSkill : ISkill
                    {
                        #region non-serilizable-motility
                        [BsonIgnore]
                        public static Dictionary<string, string> StatDescription { get; } = new Dictionary<string, string>()
            {
                { "Координация", "Наиболее близок к ловкости или точности. Определяет, можете ли вы поймать подброшенную монету, либо точно выстрелить из пистолета." },
                {"Восприятие", "Чтобы изучить улики, сперва нужно их найти. Поиск скрытых предметов, прячущихся врагов и т.п." },
                {"Скорость реакции", "Способность уклоняться от атак и ловушек, подмечать мимолетные детали, вовремя реагировать на смену окружения." },
                {"Эквалибристика", "Ваша ловкость. Двигайтесь грациозно, стильно, модно, молодежно. Это и ваша скрытность и умение танцевать." },
                {"Техника", "Умение разбираться в технике, чинить ее, использовать подручные материалы себе на пользу. Также умение эти материалы добыть, возможно не совсем законным способом, обчищая чужие карманы." },
                {"Самообладание", "Ваша способность скрывать свои эмоции, сохранять покерфейс и соответственно не ударить лицом в грязь. Позволяет еще больше понимать язык тела окружающих." }
            };
                        public Dictionary<string, string> GetStatDescription() => StatDescription;
                        [BsonIgnore]
                        public string TranslatedName => "Моторика";
                        [BsonIgnore]
                        public List<Perk> PerkList => new List<Perk> { Coordination, Perception, ReactionSpeed, SavoirFaire, Interfacing, Composure };
                        #endregion
                        public CoordinationType Coordination { get; set; } = new();
                        public PerceptionType Perception { get; set; } = new();
                        public ReactionSpeedType ReactionSpeed { get; set; } = new();
                        public SavoirFaireType SavoirFaire { get; set; } = new();
                        public InterfacingType Interfacing { get; set; } = new();
                        public ComposureType Composure { get; set; } = new();
                        #region motorics-records
                        public record CoordinationType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Координация";
                            }
                        }
                        public record PerceptionType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Восприятие";
                            }
                        }
                        public record ReactionSpeedType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Скорость реакции";
                            }
                        }
                        public record SavoirFaireType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Эквалибристика";
                            }
                        }
                        public record InterfacingType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Техника";
                            }
                        }
                        public record ComposureType : Perk
                        {
                            public override string GetTranslatedName()
                            {
                                return "Самообладание";
                            }
                        }
                        #endregion
                        public int MaxPoints { get; set; } = 6;
                    }
                }
            }
        }

        public class Shared
        {
            private readonly MemoryCache _cache;
            public Shared()
            {
                _cache = CacheSingleton.GetInstance();
            }

            public async Task SendSelectMenuForSelectPersonAsync(SocketSlashCommand command, SocketGuildUser mentionedUser, SelectMenuIds selectMenuId)
            {
                User? user = null;
                if ((user = _cache.Get<User>(mentionedUser.Id)) == null)
                {
                    var userCollection = Program.BotDB.GetCollection<User>(Program.UserCollectionName);
                    using (var cursor = await userCollection.FindAsync<User>(x => x.UserId == mentionedUser.Id))
                    {
                        user = cursor.FirstOrDefault();
                    }
                }
                if (user != null)
                {
                    if (user.Persons.Count > 0)
                    {
                        await command.RespondAsync($"Персонажи {mentionedUser.Mention}", ephemeral: true, components: new ComponentBuilder().WithSelectMenu(CreatePersonsSelectMenu(user.Persons, selectMenuId)).Build());
                    }
                    else
                    {
                        await command.RespondAsync($"У пользователя {mentionedUser.DisplayName} нет персонажей", ephemeral: true);
                    }
                }
                else
                {
                    await command.RespondAsync($"У пользователя {mentionedUser.DisplayName} нет персонажей", ephemeral: true);
                }
            }

            public async Task SendSelectMenuForSelectPersonAsync(SocketSlashCommand command, SocketGuildUser mentionedUser, SelectMenuIds selectMenuId, string message)
            {
                User? user = null;
                if ((user = _cache.Get<User>(mentionedUser.Id)) == null)
                {
                    var userCollection = Program.BotDB.GetCollection<User>(Program.UserCollectionName);
                    using (var cursor = await userCollection.FindAsync<User>(x => x.UserId == mentionedUser.Id))
                    {
                        user = cursor.FirstOrDefault();
                    }
                }
                if (user != null)
                {
                    if (user.Persons.Count > 0)
                    {
                        await command.RespondAsync(message, ephemeral: true, components: new ComponentBuilder().WithSelectMenu(CreatePersonsSelectMenu(user.Persons, selectMenuId)).Build());
                    }
                    else
                    {
                        await command.RespondAsync($"У пользователя {mentionedUser.DisplayName} нет персонажей", ephemeral: true);
                    }
                }
                else
                {
                    await command.RespondAsync($"У пользователя {mentionedUser.DisplayName} нет персонажей", ephemeral: true);
                }
            }

            public SelectMenuBuilder CreatePersonsSelectMenu(IReadOnlyList<Person> persons, SelectMenuIds selectMenuId)
            {
                SelectMenuBuilder builder = new SelectMenuBuilder();
                builder = builder.WithCustomId(selectMenuId.ToString()).WithMinValues(1).WithMaxValues(1).WithPlaceholder("Выберите персонажа").WithType(ComponentType.SelectMenu);
                int iter = 0;
                foreach (var person in persons)
                {
                    builder = builder.AddOption(person.Name, (iter++).ToString());
                }
                return builder;
            }

            public async Task<User?> GetUserFromRepositoryAsync(ulong userId)
            {
                User? user = null;
                var userCollection = Program.BotDB.GetCollection<User>(Program.UserCollectionName);
                if ((user = _cache.Get<User>(userId)) == null)
                {
                    using (var cursor = await userCollection.FindAsync<User>(x => x.UserId == userId))
                    {
                        user = cursor.FirstOrDefault();
                    }
                    if (user is not null)
                        _cache.Set<User>(userId, user, CacheSingleton.GetStandartCacheEntryOptions());
                }
                return user;
            }

            public static Perk? GetCrownedPerkOrDefault(ISkill[] skillArray)
            {
                foreach (ISkill skill in skillArray)
                {
                    foreach (Perk perk in skill.PerkList)
                    {
                        if (perk.isSpeciality)
                        {
                            return perk;
                        }
                    }
                }
                return null;
            }

            public async Task SaveToAllRepository(User user)
            {
                var userCollection = Program.BotDB.GetCollection<User>(Program.UserCollectionName);
                await userCollection.ReplaceOneAsync(x => x.UserId == user.UserId, user, new ReplaceOptions { IsUpsert = true });
                _cache.Set<User>(user.UserId, user, CacheSingleton.GetStandartCacheEntryOptions());
            }

            public static bool AreStringsClose(string str1, string str2)
            {
                bool result = false;
                if ((str1.Length > str2.Length + str2.Length / 2) || (str2.Length > str1.Length + str1.Length / 2))
                {
                    return result;
                }
                const int equalDistance = 3;
                if (FindDamerauLevenshteinDistance(str1, str2) <= equalDistance)
                {
                    result = true;
                }
                return result;
            }

            private static int FindDamerauLevenshteinDistance(string str1, string str2)
            {
                int n = str1.Length + 1;
                int m = str2.Length + 1;
                int[,] arrayDistance = new int[n, m];
                for (int i = 0; i < n; i++)
                    arrayDistance[i, 0] = i;
                for (int j = 0; j < m; j++)
                    arrayDistance[0, j] = j;
                for (int i = 1; i < n; i++)
                {
                    for (int j = 1; j < m; j++)
                    {
                        int cost = str1[i - 1] == str2[j - 1] ? 0 : 1;
                        arrayDistance[i, j] = MinOfThree(arrayDistance[i - 1, j] + 1, arrayDistance[i, j - 1] + 1, arrayDistance[i - 1, j - 1] + cost);
                        if (i > 1 && j > 1 && str1[i - 1] == str2[j - 2] && str1[i - 2] == str2[j - 1])
                        {
                            arrayDistance[i, j] = Math.Min(arrayDistance[i, j], arrayDistance[i - 2, j - 2] + cost);
                        }
                    }
                }
                return arrayDistance[n - 1, m - 1];
            }

            private static int MinOfThree(int a, int b, int c)
            {
                if (a < b)
                {
                    if (a < c)
                    {
                        return a;
                    }
                    else return c;
                }
                else if (b < c)
                {
                    return b;
                }
                else return c;
            }
        }

        public class RoleList
        {

            public async Task ShowRoleList(SocketSlashCommand command)
            {
                if (!command.HasResponded)
                {
                    await command.DeferAsync(ephemeral: true);
                }
                long choosenPerkId = 0;
                Embed[] embedMsg = new Embed[0];
                if (command.Data.Options.Count > 0)
                {
                    SocketSlashCommandDataOption option = command.Data.Options.First();
                    if (option is null)
                    {
                        return;
                    }
                    choosenPerkId = (long)option.Value;
                    if (choosenPerkId > 0)
                    {
                        if (choosenPerkId == (long)PerksId.Intelligence)
                        {
                            embedMsg = CreateEmbedMessage(new Skills.IntellectSkill());
                        }
                        else if (choosenPerkId == (long)PerksId.Mentality)
                        {
                            embedMsg = CreateEmbedMessage(new Skills.PsycheSkill());
                        }
                        else if (choosenPerkId == (long)PerksId.Physic)
                        {
                            embedMsg = CreateEmbedMessage(new Skills.PhysiqueSkill());
                        }
                        else if (choosenPerkId == (long)PerksId.Motility)
                        {
                            embedMsg = CreateEmbedMessage(new Skills.MotoricsSkill());
                        }
                        else
                        {
                            embedMsg = CreateEmbedMessage(new Skills.ISkill[] { new Skills.IntellectSkill(), new Skills.PsycheSkill(), new Skills.PhysiqueSkill(), new Skills.MotoricsSkill() });
                        }
                    }

                }
                if (embedMsg.Count() > 0)
                {
                    await command.ModifyOriginalResponseAsync(x => x.Embeds = embedMsg);
                }
                else
                    await command.ModifyOriginalResponseAsync(x => x.Content = "Error with creating respond");
            }

            Embed[] CreateEmbedMessage(Skills.ISkill perkSystem)
            {
                return CreateEmbedMessage(new Skills.ISkill[] { perkSystem });
            }
            Embed[] CreateEmbedMessage(Skills.ISkill[] perkSystem)
            {
                Embed[] embeds = new Embed[perkSystem.Length];
                try
                {

                    int embedIter = 0;
                    foreach (var perk in perkSystem)
                    {
                        EmbedBuilder embedBuilder = new EmbedBuilder();
                        embedBuilder.WithTitle("**" + perk.TranslatedName + "**");
                        StringBuilder sb = new StringBuilder();
                        var statDescription = perk.GetStatDescription();
                        foreach (var stat in statDescription)
                        {
                            sb.AppendLine("**" + stat.Key + "**");
                            sb.AppendLine(stat.Value);
                        }
                        embedBuilder.WithDescription(sb.ToString());
                        embedBuilder.WithColor(Color.DarkBlue);
                        embeds[embedIter++] = embedBuilder.Build();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }
                return embeds;
            }
        }

        public class CreationCharacter
        {

            /// <summary>
            /// Initiates creation of character. Called by SlashCommandHandler.
            /// </summary>
            /// <param name="command"></param>
            /// <returns></returns>
            public async Task CreateCharacterAsync(SocketSlashCommand command)
            {
                try
                {
                    User? user;
                    user = await new Shared().GetUserFromRepositoryAsync(command.User.Id);
                    if (user is not null)
                    {
                        if (user.Persons.Count() < 3)
                        {
                            await FillPersonInfoAsync(command);
                        }
                        else
                        {
                            await command.RespondAsync("У вас не может быть более 3 персонажей");
                        }
                    }
                    else
                    {
                        await FillPersonInfoAsync(command);
                    }

                }
                catch (MongoException mEx)
                {
                    await command.RespondAsync("Ошибка записи в базу данных");
                    Console.WriteLine(mEx.Message);
                }
                catch (Exception ex)
                {
                    await command.RespondAsync("Произошла непредвиденная ошибка");
                    Console.WriteLine(ex.Message + " " + ex.InnerException, ConsoleColor.Red);
                }
            }

            private async Task FillPersonInfoAsync(SocketSlashCommand command)
            {
                ModalBuilder modalBuilder = new ModalBuilder();
                modalBuilder.WithTitle("Создание персонажа");
                modalBuilder.WithCustomId(ModalIds.Creation.ToString());
                modalBuilder.AddTextInput("Введите имя персонажа", "name", TextInputStyle.Short, maxLength: 64, minLength: 1, required: true);
                modalBuilder.AddTextInput("Введите название коронного навыка", "crown", TextInputStyle.Short, "грубаясила", required: true);
                TextInputBuilder txtInputSkillsBuilder = new() { CustomId = "skills", Label = $"Распределите {RPGlobals.StandartFreePoints} очков навыков персонажа", Required = true, Placeholder = "Формат: [навык слитно на русском]:[значение] через пробел.\nНапример,\nэквалибристика:4 техника:2", Style = TextInputStyle.Paragraph };
                modalBuilder.AddTextInput(txtInputSkillsBuilder);
                await command.RespondWithModalAsync(modalBuilder.Build());
            }

            private void FillInfoUser(out User user, ulong id)
            {
                user = new User();
                user.UserId = id;
            }
            /// <summary>
            /// Creates new character, added it to DB and to local cache. Called my ModalHandler
            /// </summary>
            /// <param name="socketModal"></param>
            /// <returns></returns>
            public async Task GetInfoForCreateAsync(SocketModal socketModal)
            {
                var components = socketModal.Data.Components;
                User? user = null;
                Person newPerson = new();
                try
                {
                    foreach (SocketMessageComponentData component in components)
                    {
                        if (component.CustomId == "name")
                        {
                            newPerson.Name = component.Value;
                        }
                        else if (component.CustomId == "crown")
                        {
                            string inputedPerk = component.Value ?? throw new ArgumentNullException();
                            inputedPerk = inputedPerk.ToLower().Replace(" ", "");
                            Type skillType = GetSkillOfPerkByNameRus(inputedPerk, new ISkill[] { newPerson.SkillSet.Intellect, newPerson.SkillSet.Psyche, newPerson.SkillSet.Physique, newPerson.SkillSet.Motorics }, true) ?? throw new ArgumentNullException("Введеный перк не был найден");
                            if (skillType.Name == nameof(Skills.IntellectSkill))
                            {
                                newPerson.SkillSet.Intellect.MaxPoints += 1;
                            }
                            else if (skillType.Name == nameof(Skills.PsycheSkill))
                            {
                                newPerson.SkillSet.Psyche.MaxPoints += 1;
                            }
                            else if (skillType.Name == nameof(Skills.PhysiqueSkill))
                            {
                                newPerson.SkillSet.Physique.MaxPoints += 1;
                            }
                            else if (skillType.Name == nameof(Skills.MotoricsSkill))
                            {
                                newPerson.SkillSet.Motorics.MaxPoints += 1;
                            }
                            AddPointsToPerkByNameRus(inputedPerk, new ISkill[] { newPerson.SkillSet.Intellect, newPerson.SkillSet.Psyche, newPerson.SkillSet.Physique, newPerson.SkillSet.Motorics }, 1);
                        }
                        else if (component.CustomId == "skills")
                        {
                            string inputedPerk = component.Value ?? throw new ArgumentNullException();
                            Dictionary<string, int> perkValue = ComparePerksValues(inputedPerk);
                            StatCalculus(perkValue, newPerson);
                        }
                    }
                    newPerson.Health = newPerson.SkillSet.Physique.Endurance.points;
                    newPerson.Mental = newPerson.SkillSet.Psyche.Willpower.points;
                    // get the info about user
                    user = await new Shared().GetUserFromRepositoryAsync(socketModal.User.Id);
                    if (user == null)
                    {
                        FillInfoUser(out user, socketModal.User.Id);
                    }
                    if (user.Persons.Count < 3)
                    {
                        user.Persons.Add(newPerson);
                        await new Shared().SaveToAllRepository(user);
                        await socketModal.RespondAsync("Персонаж был успешно создан", ephemeral: true);
                    }
                    else
                    {
                        await socketModal.RespondAsync("У вас не может быть более 3 персонажей", ephemeral: true);
                        return;
                    }
                }
                catch (FormatException formatException)
                {
                    await socketModal.RespondAsync("Неверный формат ввода", ephemeral: true);
                    Console.WriteLine(formatException.Message);
                }
                catch (OverflowException overflowEx) when (overflowEx.Message == "PointsOverflow")
                {
                    await socketModal.RespondAsync("Итоговое значение способности не может быть выше максимума (коронный навык учтен)", ephemeral: true);
                }
                catch (OverflowException overflowEx)
                {
                    await socketModal.RespondAsync("Было введено слишком большое значение у способности", ephemeral: true);
                    Console.WriteLine(overflowEx.Message);
                }
                catch (ArgumentNullException nullEx)
                {
                    await socketModal.RespondAsync("Каждая строка должна быть непустой и введена согласно формату", ephemeral: true);
                    Console.WriteLine(nullEx.Message);
                }
                catch (CreatingCharacterException crEx)
                {
                    await socketModal.RespondAsync(crEx.Message, ephemeral: true);
                    Console.WriteLine(crEx.Message);
                }
                catch (MongoException mongoEx)
                {
                    await socketModal.RespondAsync($"Произошла ошибка при записив базу данных: {mongoEx.Message}", ephemeral: true);
                    Console.WriteLine(mongoEx.Message);
                }
                catch (Exception ex)
                {
                    await socketModal.RespondAsync($"Произошла непредвиденная ошибка: {ex.Message}");
                    Console.WriteLine(ex.Message);
                }
            }

            private Dictionary<string, int> ComparePerksValues(string inputedString)
            {
                Dictionary<string, int> perksValues = new Dictionary<string, int>();
                inputedString = inputedString.ToLower().Replace("\n", " ");
                string[] inputedStringArray = inputedString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (string pair in inputedStringArray)
                {
                    string[] elements = pair.Split(':');
                    if (elements.Length != 2)
                        throw new FormatException("Элементов, разделенных :, не может быть больше 2");
                    perksValues.Add(elements[0], Convert.ToInt32(elements[1]));
                }
                return perksValues;
            }

            private void StatCalculus(Dictionary<string, int> perkValuePair, Person person)
            {
                int count = 0;
                foreach (var perksValue in perkValuePair)
                {
                    int value = Convert.ToInt32(perksValue.Value);
                    if (value < 0)
                        throw new CreatingCharacterException("Числа не могут быть отрицательными");
                    count += value;
                    AddPointsToPerkByNameRus(perksValue.Key, new ISkill[] { person.SkillSet.Intellect, person.SkillSet.Psyche, person.SkillSet.Physique, person.SkillSet.Motorics }, value);
                }
                if (count > person.FreePoints)
                {
                    throw new CreatingCharacterException($"Количество очков, затрачиваемых на прокачку персонажа, не может превышать {person.FreePoints}");
                }
                else
                    person.FreePoints -= count;
            }

            public void AddPointsToPerkByNameRus(string perkNameRus, Skills.ISkill[] skillArray, int value)
            {
                bool isPerkFounded = false;
                foreach (Skills.ISkill skill in skillArray)
                {
                    foreach (Skills.Perk perk in skill.PerkList)
                    {
                        if (Shared.AreStringsClose(perkNameRus, perk.GetTranslatedName().ToLower().Replace(" ", "")))
                        {
                            isPerkFounded = true;
                            perk.points += value;
                            if (perk.points > skill.MaxPoints)
                                throw new OverflowException("PointsOverflow");
                            return;
                        }
                    }
                }
                if (isPerkFounded == false)
                {
                    throw new CreatingCharacterException("Необходимо внести корректные данные при выборе навыка");
                }
            }

            /// <summary>
            /// Allows get the skill type of perks
            /// </summary>
            /// <param name="perkNameRus">Name of perk in russian</param>
            /// <param name="person">Character where need to find</param>
            /// <param name="makeToCrowned">Need to make perk "crow"</param>
            /// <returns></returns>
            private Type? GetSkillOfPerkByNameRus(string perkNameRus, Skills.ISkill[] skillArray, bool makeToCrowned = false)
            {
                foreach (Skills.ISkill skill in skillArray)
                {
                    foreach (Skills.Perk perk in skill.PerkList)
                    {
                        if (Shared.AreStringsClose(perkNameRus, perk.GetTranslatedName().ToLower().Replace(" ", "")))
                        {
                            if (makeToCrowned)
                            {
                                perk.isSpeciality = true;
                            }
                            return skill.GetType();
                        }
                    }
                }
                return null;
            }
        }

        public class ShowingCharacter
        {
            public async Task ShowCharacterAsync(SocketSlashCommand command)
            {
                try
                {
                    SocketGuildUser mentionedUser = (SocketGuildUser)command.Data.Options.First().Value;
                    await new Shared().SendSelectMenuForSelectPersonAsync(command, mentionedUser, SelectMenuIds.ShowPerson);
                }
                catch (MongoException mongoEx)
                {
                    await command.RespondAsync("Произошла ошибка при доступе к базе данных. Обратитесь к поддержке");
                }
                catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException)
                {
                    await command.RespondAsync("Не было упомянуто ни одного пользователя");
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex2)
                {
                    await command.RespondAsync("Произошла непредвиденная ошибка при попытке увидеть персонажей игрока");
                    Console.WriteLine(ex2.Message + " " + ex2.InnerException);
                }
            }

            public async Task ShowingCharacterInfoAsync(SocketMessageComponent component)
            {
                try
                {
                    var mentionedUser = component.Message.MentionedUsers.First();
                    User? user = null;
                    user = await new Shared().GetUserFromRepositoryAsync(mentionedUser.Id);
                    if (user != null)
                    {
                        int numInList = Convert.ToInt32(component.Data.Values.First());
                        Embed answer = CreateAnswerEmbed(user.Persons[numInList]);
                        await component.RespondAsync(embed: answer, ephemeral: true);
                    }
                    else
                    {
                        await component.RespondAsync("Пользователь не был найден", ephemeral: true);
                    }
                }
                catch (MongoException mongoEx)
                {
                    Console.WriteLine($"{mongoEx.Message} {mongoEx.StackTrace}");
                    await component.RespondAsync("Произошла ошибка при доступе к базе данных");
                }
                catch (Exception listEx) when (listEx is ArgumentOutOfRangeException || listEx is FormatException)
                {
                    Console.WriteLine($"{listEx.Message} {listEx.StackTrace}");
                    await component.RespondAsync("Выбранный персонаж игрока уже более не существует", ephemeral: true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            private Embed CreateAnswerEmbed(in Person person)
            {
                EmbedBuilder embedBuilder = new EmbedBuilder();
                embedBuilder.WithColor(Color.DarkPurple).WithTitle($"Характеристики персонажа {person.Name}");
                embedBuilder.WithDescription(CreateDescriptionForEmbed(person));
                return embedBuilder.Build();
            }
            private string CreateDescriptionForEmbed(Person person)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"**Здоровье: {person.Health}**");
                stringBuilder.AppendLine($"**Мораль: {person.Mental}**");
                stringBuilder.AppendLine("\n**Интеллект**");
                foreach (Skills.Perk perk in person.SkillSet.Intellect.PerkList)
                {
                    AddInfoAboutPerk(stringBuilder, perk);
                }
                stringBuilder.Append("Максимальное количество очков: ");
                stringBuilder.AppendLine(person.SkillSet.Intellect.MaxPoints.ToString());

                stringBuilder.AppendLine("\n**Психика**");
                foreach (Skills.Perk perk in person.SkillSet.Psyche.PerkList)
                {
                    AddInfoAboutPerk(stringBuilder, perk);
                }
                stringBuilder.Append("Максимальное количество очков: ");
                stringBuilder.AppendLine(person.SkillSet.Psyche.MaxPoints.ToString());

                stringBuilder.AppendLine("\n**Физика**");
                foreach (Skills.Perk perk in person.SkillSet.Physique.PerkList)
                {
                    AddInfoAboutPerk(stringBuilder, perk);
                }
                stringBuilder.Append("Максимальное количество очков: ");
                stringBuilder.AppendLine(person.SkillSet.Physique.MaxPoints.ToString());

                stringBuilder.AppendLine("\n**Моторика**");
                foreach (Skills.Perk perk in person.SkillSet.Motorics.PerkList)
                {
                    AddInfoAboutPerk(stringBuilder, perk);
                }
                stringBuilder.Append("Максимальное количество очков: ");
                stringBuilder.AppendLine(person.SkillSet.Motorics.MaxPoints.ToString());
                stringBuilder.Append("\nСвободных очков на распределение: ");
                stringBuilder.AppendLine(person.FreePoints.ToString());
                return stringBuilder.ToString();
            }
            private void AddInfoAboutPerk(StringBuilder stringBuilder, Skills.Perk perk)
            {
                stringBuilder.Append(perk.GetTranslatedName());
                stringBuilder.Append(": ");
                stringBuilder.Append(perk.points.ToString());
                if (perk.isSpeciality)
                {
                    stringBuilder.Append(" *коронный*");
                }
                stringBuilder.Append("\n");
            }

        }

        public class ChangingCharacter
        {
            public async Task ChangeCharacterAsync(SocketSlashCommand command)
            {
                try
                {
                    SocketGuildUser mentionedUser = (SocketGuildUser)command.Data.Options.First().Value;
                    await new Shared().SendSelectMenuForSelectPersonAsync(command, mentionedUser, SelectMenuIds.ChangePerson);
                }
                catch (MongoException mongoEx)
                {
                    await command.RespondAsync("Произошла ошибка при доступе к базе данных. Обратитесь к поддержке");
                    Console.WriteLine(mongoEx.Message);
                }
                catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException)
                {
                    await command.RespondAsync("Не было упомянуто ни одного пользователя");
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex2)
                {
                    await command.RespondAsync("Произошла непредвиденная ошибка при попытке увидеть персонажей игрока");
                    Console.WriteLine(ex2.Message);
                }
            }

            public async Task GetInfoForChangingAsync(SocketMessageComponent component)
            {
                try
                {
                    var mentionedUser = component.Message.MentionedUsers.First();
                    User? user = null;
                    user = await new Shared().GetUserFromRepositoryAsync(mentionedUser.Id);
                    if (user != null)
                    {
                        SocketGuildUser guildUser = (SocketGuildUser)component.User;
                        bool getAboutPointsInfo = false;
                        if (guildUser.Roles.FirstOrDefault(x => x.Permissions.KickMembers) != null)
                        {
                            getAboutPointsInfo = true;
                        }
                        int numPers = Convert.ToInt32(component.Data.Values.First());
                        Modal respondModal = CreateChangingModal(user.Persons[numPers], mentionedUser.Id, getAboutPointsInfo, numPers);
                        await component.RespondWithModalAsync(respondModal);
                    }
                    else
                    {
                        await component.RespondAsync("У указанного пользователя нет персонажей", ephemeral: true);
                    }
                }
                catch (System.NotSupportedException notSup)
                {
                    Console.WriteLine($"{notSup.Message}");
                }
                catch (MongoException mongoEx)
                {
                    await component.RespondAsync("Произошла ошибка при доступе к базе данных");
                    Console.WriteLine(mongoEx.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + " " + ex.StackTrace + "\nType: " + ex.GetType().ToString());
                    await component.RespondAsync("Произошла непредвиденная ошибка");
                }
            }

            private Modal CreateChangingModal(Person person, ulong userID, bool getPointsInfo, int personCount)
            {
                ModalBuilder modalBuilder = new ModalBuilder();
                modalBuilder = modalBuilder.WithTitle("Изменение персонажа");
                modalBuilder = modalBuilder.WithCustomId(ModalIds.Changing.ToString());
                modalBuilder = modalBuilder.AddTextInput("Введите имя персонажа", $"name_{userID}", TextInputStyle.Short, maxLength: 64, minLength: 1, required: false, value: person.Name);
                if (getPointsInfo)
                {
                    modalBuilder = modalBuilder.AddTextInput("Использовать свободные очки?", "points", TextInputStyle.Short, placeholder: "Да или нет", required: true, value: "нет");
                }

                TextInputBuilder txtInputSkillsBuilder = new() { CustomId = $"skills_{personCount}", Label = $"Добавьте нужное кол-во очков (доступно: {person.FreePoints})", Required = true, Placeholder = "Формат: [навык слитно на русском]:[значение] через пробел.\nНапример,\nэквалибристика:4 моторика:2", Style = TextInputStyle.Paragraph };
                modalBuilder = modalBuilder.AddTextInput(txtInputSkillsBuilder);
                return modalBuilder.Build();
            }

            public async Task ChangingDataPersonAsync(SocketModal socketModal)
            {
                try
                {
                    ulong userId = ulong.MinValue;
                    string name = "", skills = "";
                    bool usePoints = true;
                    int numPers = int.MinValue;
                    foreach (SocketMessageComponentData data in socketModal.Data.Components)
                    {
                        if (data.CustomId.StartsWith("name"))
                        {
                            userId = Convert.ToUInt64(data.CustomId.Substring(data.CustomId.IndexOf("_") + 1));
                            name = data.Value;
                        }
                        else if (data.CustomId.StartsWith("skills"))
                        {
                            numPers = Convert.ToInt32(data.CustomId.Substring(data.CustomId.IndexOf("_") + 1));
                            skills = data.Value;
                        }
                        else if (data.CustomId == "points")
                        {
                            if (data.Value.ToLower() == "нет")
                            {
                                usePoints = false;
                            }
                        }
                    }
                    if (userId == ulong.MinValue || numPers == int.MinValue)
                        throw new ArgumentException("UserID or number of person cannot be min value");
                    User? user;
                    user = await new Shared().GetUserFromRepositoryAsync(userId);
                    if (user == null)
                        throw new ArgumentException("У пользователя нет персонажей");
                    int oldWillpower = user.Persons[numPers].SkillSet.Psyche.Willpower.points;
                    int oldEndurance = user.Persons[numPers].SkillSet.Physique.Endurance.points;
                    user.Persons[numPers] = CalculatePerson(user, numPers, name, skills, usePoints);
                    user.Persons[numPers].Mental += user.Persons[numPers].SkillSet.Psyche.Willpower.points - oldWillpower;
                    user.Persons[numPers].Health += user.Persons[numPers].SkillSet.Physique.Endurance.points - oldEndurance;
                    await new Shared().SaveToAllRepository(user);
                    await socketModal.RespondAsync("Изменения успешно сохранены", ephemeral: true);
                }
                catch (ChangingCharacterException changeEx)
                {
                    await socketModal.RespondAsync(changeEx.Message, ephemeral: true);
                    Console.WriteLine(changeEx.Message);
                }
                catch (ArgumentException argEx)
                {
                    await socketModal.RespondAsync(argEx.Message);
                    Console.WriteLine(argEx.Message);
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    await socketModal.RespondAsync("Некорректный ввод", ephemeral: true);
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    await socketModal.RespondAsync("Произошла непредвиденная ошибка");
                    Console.WriteLine(ex.Message);
                }
            }

            private Person CalculatePerson(User user, int numPers, string name, string skills, bool usePoints)
            {
                Person changingPerson = user.Persons[numPers];
                changingPerson.Name = name;
                StatCalculus(ComparePerksValues(skills), changingPerson, usePoints);
                return changingPerson;
            }

            private Dictionary<string, int> ComparePerksValues(string inputedString)
            {
                Dictionary<string, int> perksValues = new Dictionary<string, int>();
                inputedString = inputedString.ToLower().Replace("\n", " "); ;
                string[] inputedStringArray = inputedString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (string pair in inputedStringArray)
                {
                    string[] elements = pair.Split(':');
                    if (elements.Length != 2)
                        throw new FormatException("Элементов, разделенных :, не может быть больше 2");
                    perksValues.Add(elements[0], Convert.ToInt32(elements[1]));
                }
                return perksValues;
            }

            private void StatCalculus(Dictionary<string, int> perkValuePair, Person person, bool usePoints)
            {
                if (usePoints)
                {
                    int count = 0;
                    foreach (var perksValue in perkValuePair)
                    {
                        int value = Convert.ToInt32(perksValue.Value);
                        count += value;
                    }
                    if (count > person.FreePoints)
                    {
                        throw new ChangingCharacterException($"Количество очков, затрачиваемых на прокачку персонажа, не может превышать {person.FreePoints}");
                    }
                    else
                        person.FreePoints -= count;
                }
                foreach (var perksValue in perkValuePair)
                {
                    int value = Convert.ToInt32(perksValue.Value);
                    AddPointsToPerkByNameRus(perksValue.Key, new ISkill[] { person.SkillSet.Intellect, person.SkillSet.Psyche, person.SkillSet.Physique, person.SkillSet.Motorics }, value, usePoints);
                }

            }

            private void AddPointsToPerkByNameRus(string perkNameRus, Skills.ISkill[] skillArray, int value, bool usePoints)
            {
                bool isPerkFounded = false;
                if (usePoints)
                {
                    foreach (Skills.ISkill skill in skillArray)
                    {
                        bool founded = false;
                        foreach (Skills.Perk perk in skill.PerkList)
                        {
                            if (Shared.AreStringsClose(perkNameRus, perk.GetTranslatedName().ToLower().Replace(" ", "")))
                            {
                                int newPoints = perk.points + value;
                                if (newPoints > skill.MaxPoints)
                                    throw new ChangingCharacterException($"Количество добавляемых очков в этот навык не может быть ниже 1 и больше {skill.MaxPoints}");
                                founded = true;
                                break;
                            }
                        }
                        if (founded)
                            break;
                    }
                }
                foreach (Skills.ISkill skill in skillArray)
                {
                    foreach (Skills.Perk perk in skill.PerkList)
                    {
                        if (Shared.AreStringsClose(perkNameRus, perk.GetTranslatedName().ToLower().Replace(" ", "")))
                        {
                            isPerkFounded = true;
                            perk.points += value;
                            return;
                        }
                    }
                }
                if (isPerkFounded == false)
                {
                    throw new ChangingCharacterException("Необходимо внести корректные данные при выборе навыка");
                }
            }
        }

        public class RollPerks
        {
            public async Task RollingPerksAsync(SocketSlashCommand command)
            {
                try
                {
                    User? user = await new Shared().GetUserFromRepositoryAsync(command.User.Id);
                    ArgumentNullException.ThrowIfNull(user, "User cannot be null");
                    var skillName = command.Data.Options.First().Name;
                    string perkName = (string)command.Data.Options.First().Options.First().Value;
                    Int64? modifier = null;
                    if (command.Data.Options.First().Options.Count == 2)
                    {
                        modifier = (Int64)command.Data.Options.First().Options.First(x => x.Value is Int64).Value;
                    }
                    if (user.Persons.Count > 1)
                    {
                        var selectMenuBuilder = new Shared().CreatePersonsSelectMenu(user.Persons, SelectMenuIds.RollPerson);
                        string message = $"Выберите персонажа, характеристики которого вы хотите использовать. Навык: " + perkName + ".";
                        if (modifier != null)
                        {
                            message = message + $" Модификатор: {modifier}";
                        }
                        await command.RespondAsync(message, ephemeral: true, components: new ComponentBuilder().WithSelectMenu(selectMenuBuilder).Build());
                    }
                    else if (user.Persons.Count == 1)
                    {
                        await command.RespondAsync(embed: GetRolledPerkEmbed(user.Persons[0], perkName, modifier));
                    }
                    else
                        throw new ArgumentNullException(nameof(user.Persons));
                }
                catch (InvalidCastException invCastEx)
                {
                    await command.RespondAsync("Произошла ошибка с приведением типов");
                    Console.WriteLine(invCastEx.Message);
                }
                catch (ArgumentNullException nullEx)
                {
                    await command.RespondAsync("У вас нет персонажей, либо ошибка на стороне сервера");
                    Console.WriteLine(nullEx.Message);
                }
                catch (OverflowException overEx)
                {
                    Console.WriteLine(overEx.Message);
                    await command.RespondAsync("Введеное число было слишком большим");
                }
                catch (Exception e)
                {
                    await command.RespondAsync("Произошла непредвиденная ошибка");
                    Console.WriteLine(e.Message);
                }
            }

            public async Task ChoosePersonAsync(SocketMessageComponent component)
            {
                try
                {
                    string[] messageSplit = component.Message.Content.Split(".");
                    long? modifier = null;
                    string perkName = messageSplit[1].Remove(0, "Навык: ".Length);
                    if (messageSplit.Count() == 3 && !string.IsNullOrEmpty(messageSplit[2]))
                    {
                        modifier = Convert.ToInt64(messageSplit[2].Remove(0, "Модификатор: ".Length));
                    }
                    int numPers = Convert.ToInt32(component.Data.Values.First());
                    User? user = await new Shared().GetUserFromRepositoryAsync(component.User.Id);
                    ArgumentNullException.ThrowIfNull(user, nameof(user));
                    if (user.Persons.Count == 0)
                        throw new ArgumentNullException(nameof(user.Persons));
                    await component.RespondAsync(embed: GetRolledPerkEmbed(user.Persons[numPers], perkName, modifier), ephemeral: false);
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine(ex.Message);
                    await component.RespondAsync("У пользователя нет персонажей");
                }
                catch (ArgumentException argEx)
                {
                    Console.WriteLine(argEx.Message);
                    await component.RespondAsync(argEx.Message);
                }
                catch (InvalidCastException invCastEx)
                {
                    Console.WriteLine(invCastEx.Message);
                    await component.RespondAsync("Неверный формат модификатора");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            private Embed GetRolledPerkEmbed(Person person, string perkNameRus, long? modifier)
            {
                string formattedPerkNameRus = perkNameRus.TrimStart();
                int? perkModifier = null;
                foreach (ISkill skill in new ISkill[] { person.SkillSet.Intellect, person.SkillSet.Psyche, person.SkillSet.Physique, person.SkillSet.Motorics })
                {
                    foreach (Perk perk in skill.PerkList)
                    {
                        if (formattedPerkNameRus == perk.GetTranslatedName())
                        {
                            perkModifier = perk.points;
                            break;
                        }
                    }
                    if (perkModifier != null)
                        break;
                }
                if (perkModifier == null)
                {
                    throw new ArgumentException("Модификатор перка не был найден");
                }
                EmbedBuilder embedBuilder = new EmbedBuilder();
                embedBuilder.WithTitle($"Проверка навыка {formattedPerkNameRus.ToLower()} {perkModifier} {((modifier != null) ? $"с модификатором {modifier}" : "")} для персонажа {person.Name}");
                int cubeValueFirst = Random.Shared.Next(1, 7), cubeValueSecond = Random.Shared.Next(1, 7);
                long res = cubeValueFirst + cubeValueSecond + (int)perkModifier;
                if (modifier is not null)
                    res += (long)modifier;
                string message = $"**{cubeValueFirst}** + **{cubeValueSecond}** = {cubeValueFirst + cubeValueSecond} + **{perkModifier}**{(modifier != null ? $" + {modifier}" : "")} = {res}";
                if (cubeValueFirst == 6 && cubeValueSecond == 6)
                {
                    message += "\nКритический успех!";
                }
                else if (cubeValueFirst + cubeValueSecond == 2)
                {
                    message += "\nКритическая неудача!";
                }
                embedBuilder.WithDescription(message).WithColor(Color.DarkTeal).WithCurrentTimestamp();
                return embedBuilder.Build();
            }

        }

        public class Delete
        {
            public async Task ChooseDeletePerson(SocketSlashCommand command)
            {
                try
                {
                    var mentionedUser = command.Data.Options.FirstOrDefault()?.Value as SocketGuildUser;
                    //if (mentionedUser == null)
                    //    throw new ArgumentException("Необходимо упомянуть пользователя");
                    var guildUser = (SocketGuildUser)command.User;
                    string message = $"Персонажи {mentionedUser?.Mention ?? command.User.Mention}. Выберите одного для удаления";
                    if (mentionedUser != null && mentionedUser.Id != command.User.Id)
                    {
                        if (guildUser.Roles.FirstOrDefault(x => x.Permissions.KickMembers) == null)
                        {
                            throw new ArgumentException("Не имея прав выгнать пользователя, вы не можете удалять чужих персонажей");
                        }
                    }

                    await new Shared().SendSelectMenuForSelectPersonAsync(command, mentionedUser ?? guildUser, SelectMenuIds.DeletePerson, message);
                }
                catch (InvalidCastException invCastEx)
                {
                    Console.WriteLine(invCastEx.Message);
                    await command.RespondAsync(invCastEx.Message);
                }
                catch (ArgumentOutOfRangeException argOutEx)
                {
                    Console.WriteLine(argOutEx.Message);
                    await command.RespondAsync("У пользователя нет выбранного персонажа. Ошибка: " + argOutEx.Message);
                }
                catch (ArgumentException argEx)
                {
                    Console.WriteLine(argEx.Message);
                    await command.RespondAsync(argEx.Message);
                }
                catch (FormatException formEx)
                {
                    Console.WriteLine(formEx.Message);
                    await command.RespondAsync(formEx.Message);
                }
                catch (Exception ex)
                {

                }
            }

            public async Task DeletePerson(SocketMessageComponent selectMenu)
            {
                int personCount = Convert.ToInt32(selectMenu.Data.Values.First());
                SocketUser mentionedUser = selectMenu.Message.MentionedUsers.First();
                User user = await new Shared().GetUserFromRepositoryAsync(mentionedUser.Id) ?? throw new ArgumentNullException("Пользователя нет в базе данных");
                user.Persons.RemoveAt(personCount);
                await new Shared().SaveToAllRepository(user);
                await selectMenu.UpdateAsync(x =>
                {
                    x.Content = "Персонаж был удален";
                    x.Components = null;
                });
            }
        }

        public class Give
        {
            public async Task GivePoints(SocketSlashCommand command)
            {
                try
                {
                    ArgumentNullException.ThrowIfNull(((SocketGuildUser)command.User).Roles.FirstOrDefault(x => x.Permissions.KickMembers), nameof(User));
                    SocketGuildUser mentionedUser = (SocketGuildUser)command.Data.Options.First().Options.First(x => x.Value is SocketUser).Value;
                    User? user = await new Shared().GetUserFromRepositoryAsync(mentionedUser.Id);
                    ArgumentNullException.ThrowIfNull(user, "Упомянутый пользователь не имеет персонажей");
                    var subcommandName = command.Data.Options.First().Name;
                    string attribute = (subcommandName != "health" && subcommandName != "moral") ? (string)command.Data.Options.First().Options.First(x => x.Value is string).Value : subcommandName;
                    Int64 value = (Int64)command.Data.Options.First().Options.First(x => x.Value is Int64).Value;
                    if (user.Persons.Count > 1)
                    {
                        var selectMenuBuilder = new Shared().CreatePersonsSelectMenu(user.Persons, SelectMenuIds.GivePoints);
                        string message = $"Выберите персонажа, характеристики которого вы хотите изменить. Атрибут: " + attribute + $". Значение {value}. Пользователь: {mentionedUser.Mention}";
                        await command.RespondAsync(message, ephemeral: true, components: new ComponentBuilder().WithSelectMenu(selectMenuBuilder).Build());
                    }
                    else if (user.Persons.Count == 1)
                    {
                        ChangeValueInPerson(user.Persons[0], (int)value, attribute);
                        await new Shared().SaveToAllRepository(user);
                        await command.RespondAsync("Изменение прошло успешно", ephemeral: true);
                    }
                    else
                        throw new ArgumentNullException(nameof(user.Persons), "У пользователя нет персонажей");
                }
                catch (InvalidCastException invCastEx)
                {
                    Console.WriteLine(invCastEx.Message);
                    await command.RespondAsync($"Произошла ошибка с приведением типов {invCastEx.Message}");
                }
                catch (ArgumentNullException nullEx) when (String.Equals(nullEx.ParamName, nameof(User)))
                {
                    Console.WriteLine(nullEx.Message);
                    await command.RespondAsync($"Недостаточно прав для исполнения данной команды");
                }
                catch (ArgumentNullException nullEx)
                {
                    Console.WriteLine(nullEx.Message);
                    await command.RespondAsync($"Ошибка некорректного аргумента: {nullEx.Message}");
                }
                catch (ArgumentException argEx)
                {
                    Console.WriteLine(argEx.Message);
                    await command.RespondAsync($"Ошибка некорректного аргумента: {argEx.Message}");
                }
                catch (OverflowException overEx)
                {
                    Console.WriteLine(overEx.Message);
                    await command.RespondAsync("Введеное число было слишком большим");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await command.RespondAsync("Произошла непредвиденная ошибка");
                }
            }

            public async Task GetPersonFromSelectMenu(SocketMessageComponent selectMenu)
            {
                try
                {
                    string message = selectMenu.Message.Content;
                    string[] submessages = message.Split('.');
                    string attribute = submessages[1].Substring(" Атрибут: ".Length);
                    long value = Convert.ToInt64(submessages[2].Substring(" Значение ".Length));
                    SocketGuildUser mentionedUser = (SocketGuildUser)selectMenu.Message.MentionedUsers.First();
                    User? user = await new Shared().GetUserFromRepositoryAsync(mentionedUser.Id);
                    ArgumentNullException.ThrowIfNull(user, nameof(user));
                    int numPers = Convert.ToInt32(selectMenu.Data.Values.First());
                    ChangeValueInPerson(user.Persons[numPers], (int)value, attribute);
                    await new Shared().SaveToAllRepository(user);
                    await selectMenu.RespondAsync("Изменение прошло успешно", ephemeral: true);
                }
                catch (ArgumentNullException nullEx)
                {
                    Console.WriteLine(nullEx.Message);
                    await selectMenu.RespondAsync($"Пользователь не найден, либо ошибка на стороне сервера: {nullEx.Message}");
                }
                catch (OverflowException overEx)
                {
                    Console.WriteLine(overEx.Message);
                    await selectMenu.RespondAsync($"Произошла ошибка переполнения: {overEx.Message}");
                }
                catch (InvalidCastException invCastEx)
                {
                    Console.WriteLine(invCastEx.Message);
                    await selectMenu.RespondAsync($"Произошла ошибка приведения типов: {invCastEx.Message}");
                }
                catch (FormatException formEx)
                {
                    Console.WriteLine(formEx.Message);
                    await selectMenu.RespondAsync($"Ошибка формата: {formEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await selectMenu.RespondAsync($"Произошла непредвиденная ошибка: {ex.Message}");
                }

            }

            private void ChangeValueInPerson(Person person, int value, string attribute)
            {
                if (Regex.IsMatch(attribute, "health"))
                {
                    person.Health += value;
                }
                else if (Regex.IsMatch(attribute, "moral"))
                {
                    person.Mental += value;
                }
                else
                {
                    foreach (ISkill skill in new ISkill[] { person.SkillSet.Intellect, person.SkillSet.Psyche, person.SkillSet.Physique, person.SkillSet.Motorics })
                    {
                        foreach (Perk perk in skill.PerkList)
                        {
                            if (String.Equals(perk.GetTranslatedName(), attribute, StringComparison.OrdinalIgnoreCase))
                            {
                                perk.points += value;
                                return;
                            }
                        }
                    }
                    throw new ArgumentException("Attribute was not founded");
                }

            }

        }
    }

}
