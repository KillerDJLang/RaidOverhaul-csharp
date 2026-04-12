using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace RaidOverhaulMain.Models;

public static class DevModels
{
    public static readonly Dictionary<string, List<string>> AmmoStackList = new()
    {
        {
            "RifleList",
            new List<string>
            {
                "5cadf6e5ae921500113bb973",
                "56dff3afd2720bba668b4567",
                "56dff0bed2720bb0668b4567",
                "59e6542b86f77411dc52a77a",
                "59e6658b86f77411d949b250",
                "56dff4a2d2720bbd668b456a",
                "5cadf6ddae9215051e1c23b2",
                "56dff338d2720bbd668b4569",
                "56dff4ecd2720b5f5a8b4568",
                "56dff216d2720bbd668b4568",
                "5c0d5e4486f77478390952fe",
                "56dff026d2720bb8668b4567",
                "59e655cb86f77411dc52a77b",
                "56dff061d2720bb5668b4567",
                "5f0596629e22f464da6bbdd9",
                "5cadf6eeae921500134b2799",
                "61962b617c6c7b169525f168",
                "56dfef82d2720bbd668b4567",
                "5fbe3ffdf8b6a877a729ea82",
                "59e0d99486f7744a32234762",
                "56dff2ced2720bb4668b4567",
                "56dff421d2720b5f5a8b4567",
                "64b7af434b75259c590fa893",
                "57a0dfb82459774d3078b56c",
                "59e68f6f86f7746c9f75e846",
                "59e690b686f7746c9f75e848",
                "5c0d5ae286f7741e46554302",
                "64b7af5a8532cf95ee0a0dbd",
                "6529243824cbe3c74a05e5c1",
                "59e4cf5286f7741778269d8a",
                "59e6920f86f77411d82aa167",
                "5656d7c34bdc2d9d198b4587",
                "59e4d24686f7741776641ac7",
                "59e6927d86f77411da468256",
                "59e6906286f7746c9f75e847",
                "64b8725c4b75259c590fa899",
                "6529302b8c26af6326029fb7",
                "54527a984bdc2d4e668b4567",
                "6196365d58ef8c428c287da1",
                "59e4d3d286f774176a36250a",
                "601949593ae8f707c4608daa",
                "64b7af734b75259c590fa895",
                "6196364158ef8c428c287d9f",
                "60194943740c5d77f6705eea",
                "59e6918f86f7746c9f75e849",
                "619636be6db0f2477964e710",
                "61962d879bb3d20b0946d385",
                "54527ac44bdc2d36668b4567",
                "601aa3d2b2bcb34913271e6d",
                "5fd20ff893a8961fc660a954",
                "6241c316234b593b5676b637",
                "57a0e5022459774d1673f889",
                "5c0d688c86f77413ae3407b2",
                "6576f96220d53a5b8f3e395e",
                "5c0d668f86f7747ccb7f13b2",
                "6628185208dd86f969db7e03",
                "662818a23a552da6aef8fada",
            }
        },
        {
            "ShotgunList",
            new List<string>
            {
                "5d6e68b3a4b9361bca7e50b5",
                "5a38ebd9c4a282000d722a5b",
                "5d6e68c4a4b9361b93413f79",
                "64b8ee384b75259c590fa89b",
                "5d6e6772a4b936088465b17c",
                "5c0d591486f7744c505b416f",
                "5d6e69c7a4b9360b6c0d54e4",
                "58820d1224597753c90aeb13",
                "5d6e6a5fa4b93614ec501745",
                "5d6e68a8a4b9360b6c0d54e2",
                "5d6e68dea4b9361bcc29e659",
                "5d6e6911a4b9361bd5780d52",
                "5d6e68e6a4b9361c140bcfe0",
                "5d6e6806a4b936088465b17e",
                "5d6e6a05a4b93618084f58d0",
                "5d6e6869a4b9361c140bcfde",
                "5d6e69b9a4b9361bc8618958",
                "5d6e6891a4b9361bd473feea",
                "5d6e695fa4b936359b35d852",
                "5d6e689ca4b9361bc8618956",
                "5d6e68d1a4b93622fe60e845",
                "5d6e6a42a4b9364f07165f52",
                "560d5e524bdc2d25448b4571",
                "5d6e67fba4b9361bc73bc779",
                "5d6e6a53a4b9361bd473feec",
                "660137ef76c1b56143052be8",
                "6601380580e77cfd080e3418",
                "660137d8481cc6907a0c5cda",
            }
        },
        {
            "SmgList",
            new List<string>
            {
                "5efb0cabfb3e451d70735af5",
                "56d59d3ad2720bdb418b4577",
                "5735ff5c245977640e39ba7e",
                "5cc80f38e4a949001152b560",
                "5ba26844d4351e00334c9475",
                "5e81f423763d9f754677bf2e",
                "5efb0d4f4bc50b58e81710f3",
                "5ba26835d4351e0035628ff5",
                "573601b42459776410737435",
                "5ea2a8e200685063ec28c05a",
                "5ba2678ad4351e44f824b344",
                "5cc80f67e4a949035e43bbba",
                "5efb0fc6aeb21837e749c801",
                "5ba26812d4351e003201fef1",
                "64b6979341772715af0f9c39",
                "573603562459776430731618",
                "573603c924597764442bd9cb",
                "57371f2b24597761224311f1",
                "6576f4708ca9c4381d16cd9d",
                "5cc80f79e4a949033c7343b2",
                "5a3c16fe86f77452b62de32a",
                "5735fdcd2459776445391d61",
                "5a269f97c4a282000b151807",
                "5737207f24597760ff7b25f2",
                "573602322459776445391df1",
                "6576f93989f0062e741ba952",
                "5cc86832d7f00c000d3a6e6c",
                "5737218f245977612125ba51",
                "5c3df7d588a4501f290594e5",
                "5cc80f53e4a949000e1ea4f8",
                "62330c18744e5e31df12f516",
                "57371eb62459776125652ac1",
                "5c925fa22e221601da359b7b",
                "57371b192459775a9f58a5e0",
                "57372140245977611f70ee91",
                "57371e4124597760ff7b25f1",
                "573718ba2459775a75491131",
                "5efb0e16aeb21837e749c7ff",
                "58864a4f2459770fcc257101",
                "64b7bbb74b75259c590fa897",
                "5996f6d686f77467977ba6cc",
                "5737201124597760fc4431f1",
                "63b35f281745dd52341e5da7",
                "57371aab2459775a77142f22",
                "573720e02459776143012541",
                "5cc80f8fe4a949033b0224a2",
                "5c0d56a986f774449d5de529",
                "5736026a245977644601dc61",
                "57371f8d24597761006c6a81",
                "5cc86840d7f00c002412c56c",
                "573719df2459775a626ccbc2",
                "573719762459775a626ccbc1",
                "5efb0da7a29a85116f6ea05f",
                "5a26ac06c4a282000c5a90a8",
                "66ec2aa6daf127599c0c31f1",
                "67654a6759116d347b0bfb86",
                "5943d9c186f7745a13413ac9",
                "62330bfadc5883093563729b",
                "62330b3ed4dc74626d570b95",
                "62330c40bdd19b369e1e53d1",
                "5996f6cb86f774678763a6ca",
                "5a26ac0ec4a28200741e1e18",
                "5996f6fc86f7745e585b4de3",
                "5a26abfac4a28232980eabff",
                "5485a8684bdc2da71d8b4567",
                "662808ec26a8e83120bb25fe",
                "662809f445b5ff428e21ac0a",
                "66280a30d3b6f288cb6b9653",
            }
        },
        {
            "SniperList",
            new List<string>
            {
                "5fc382a9d724d907e2077dab",
                "64b8f7c241772715af0f9c3d",
                "5fc382c1016cce60e8341b20",
                "560d61e84bdc2da74d8b4571",
                "5a6086ea4f39f99cd479502f",
                "5fc275cf85fd526b824a571a",
                "59e77a2386f7742ee578960a",
                "58dd3ad986f77403051cba8f",
                "5e023e88277cce2b522ff2b1",
                "64b8f7b5389d7ffd620ccba2",
                "5887431f2459777e1612938f",
                "5efb0c1bd79ff02a1f5e68d9",
                "5e023e6e34d52a55c3304f71",
                "5e023cf8186a883be655e54f",
                "5e023d48186a883be655e551",
                "5fc382b6d6fa9c00c571bbc3",
                "5e023e53d4353e3302577c4c",
                "64b8f7968532cf95ee0a0dbf",
                "5e023d34e8a400319a28ed44",
                "5a608bf24f39f98ffc77720e",
                "6768c25aa7b238f14a08d3f6",
                "66281ab7fca966e5021f81b5",
                "66281ac038f9aebf6f914138",
            }
        },
        {
            "UbglList",
            new List<string>
            {
                "5ede4739e0350d05467f73e8",
                "5ede47641cf3836a88318df1",
                "5ede474b0c226a66f5402622",
                "5d70e500a4b9364de70d38ce",
                "5f0c892565703e5c461894e9",
                "5656eb674bdc2d35148b457c",
                "5ede47405b097655935d7d16",
                "5ede475339ee016e8c534742",
                "5ede475b549eed7c6d5c18fb",
            }
        },
    };

    public static readonly List<string> ShopBlacklist = new()
    {
        "54490a4d4bdc2dbc018b4573",
        "544a3d0a4bdc2d1b388b4567",
        "55d617094bdc2d89028b4568",
        "590de52486f774226a0c24c2",
        "5648b62b4bdc2d9d488b4585",
        "544a3f024bdc2d1d388b4568",
        "590de4a286f77423d9312a32",
        "5751961824597720a31c09ac",
        "63b35f281745dd52341e5da7",
        "5996f6cb86f774678763a6ca",
        "5996f6d686f77467977ba6cc",
        "5943d9c186f7745a13413ac9",
        "5cdeb229d7f00c000e7ce174",
        "5996f6fc86f7745e585b4de3",
        "5ae083b25acfc4001a5fc702",
        "5e85aac65505fa48730d8af2",
        "58ac60eb86f77401897560ff",
        "59e8936686f77467ce798647",
        "56e294cdd2720b603a8b4575",
        "5d53f4b7a4b936793d58c780",
        "6241c316234b593b5676b637",
        "5e99735686f7744bfc4af32c",
        "62811d61578c54356d6d67ea",
        "6281214c1d5df4475f46a33a",
        "6281215b4fa03b6b6c35dc6c",
        "628121651d5df4475f46a33c",
        "628120415631d45211793c99",
        "628120f210e26c1f344e6558",
        "5ede47641cf3836a88318df1",
        "5d70e500a4b9364de70d38ce",
        "624c0570c9b794431568f5d5",
        "624c09cfbc2e27219346d955",
        "624c09da2cec124eb67c1046",
        "624c09e49b98e019a3315b66",
        "5cffa483d7ad1a049e54ef1c",
        "5f647fd3f6e4ab66c82faed6",
        "5671446a4bdc2d97058b4569",
        "61a4cda622af7f4f6a3ce617",
        "6087e570b998180e9f76dc24",
        "5efdafc1e70b5e33f86de058",
        "63dbd45917fff4dee40fe16e",
        "648c1a965043c4052a4f8505",
        "5ae089fb5acfc408fb13989b",
        "6241c2c2117ad530666a5108",
        "5580239d4bdc2de7118b4583",
        "66da1b49099cf6adcc07a36b",
        "66da1b546916142b3b022777",
        "670ad7f1ad195290cd00da7a",
        "66ec2aa6daf127599c0c31f1",
        "67654a6759116d347b0bfb86",
        "5751916f24597720a27126df",
        "57518f7724597720a31c09ab",
        "57518fd424597720c85dbaaa",
        "5a043f2c86f7741aa57b5145",
        "5a0448bc86f774736f14efa8",
        "67ade494d748873e5f0161df",
        "679baa2c61f588ae2b062a24",
        "679baa4f59b8961f370dd683",
        "679baa5a59b8961f370dd685",
        "679baa9091966fe40408f149",
        "67ab3d4b83869afd170fdd3f",
        "6628f31d2e4dbf64427b378d",
        "6628f33d585102d8bc5d75db",
        "6628f534d238d16e71d27601",
        "6628f5244c3764ecf474f23c",
        "6628f529b9886fa300b47de7",
        "6628f511ea0ab10dd0c5e2d4",
        "6628f52f246a7106c3c3dd22",
        "6628f51c8df290deed8983e9",
        "6628f531ae7642ade7384366",
        "6628f51e54b4f59a7ade4ef9",
        "6628f52b821b61722b245c10",
        "6628f5191c445ab1b8b8cdf5",
        "6628f38835c496a91efdef37",
        "6628f38262cab98c01ffada1",
        "6628f37eb50e789115223da9",
        "6628f77390fd0c39ebfa1125",
        "6628f7750e470be83e65dc76",
        "6628f7786fbce2d268af4913",
        "6628f887f6ec08df7b7dc336",
        "6628f88aaa160dcd09a782e7",
        "6628f88df6034dfe1933f636",
        "6628f973b21453a8afc0db68",
        "6628f979c3da61a625d97d69",
        "6628f977030da8d4c028f5c7",
        "6621b28d9411498998d408c3",
        "6628f3786c4f8a10a65adee4",
        "6628f76df1a913e3afc16360",
        "6628f50c78434b5effe5ced1",
        "6628f8813e3fe94f5f035010",
        "6628f2e31084cf4d62b2b40b",
        "6628f96fd59ab54dedb55801",
    };

    public static readonly List<string> ShopSpecialItems = new()
    {
        "6389c92d52123d5dd17f8876",
        "6389c8c5dbfd5e4b95197e6b",
        "59faff1d86f7746c51718c9c",
        "57347ca924597744596b4e71",
        "5d03775b86f774203e7e0c4b",
        "5a1eaa87fcdbcb001865f75e",
        "63fc44e2429a8a166c7f61e6",
        "6478641c19d732620e045e17",
        "5d1b5e94d7ad1a2b865a96b0",
        "5c94bbff86f7747ee735c08f",
        "5780cf7f2459777de4559322",
        "5d80c60f86f77440373c4ece",
        "5ede7a8229445733cb4c18e2",
        "5d80c62a86f7744036212b3f",
        "62987dfc402c7f69bf010923",
        "63a3a93f8a56922e82001f5d",
        "64ccc25f95763a1ae376e447",
        "6389c7750ef44505c87f5996",
        "6389c8fb46b54c634724d847",
    };

    public static readonly Dictionary<MongoId, Preset> CustomPresetMaps = new()
    {
        {
            "67801a9b24d426632e8efc1e",
            new Preset
            {
                Id = "67801a9b24d426632e8efc1e",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "MCM4 57",
                Parent = "3a53768e6ebfca707bf25f2e",
                Encyclopedia = "6628f50c78434b5effe5ced1",
                Items = new List<Item>
                {
                    new Item
                    {
                        Id = "3a53768e6ebfca707bf25f2e",
                        Template = "6628f50c78434b5effe5ced1",
                        Upd = new Upd
                        {
                            Repairable = new UpdRepairable { MaxDurability = 100, Durability = 100 },
                            FireMode = new UpdFireMode { FireMode = "single" },
                        },
                    },
                    new Item
                    {
                        Id = "bae8bb9021f7a8a7e409ace5",
                        Template = "63f5feead259b42f0b4d6d0f",
                        ParentId = "3a53768e6ebfca707bf25f2e",
                        SlotId = "mod_pistol_grip",
                    },
                    new Item
                    {
                        Id = "b443995ed7ed5ac6f455c8cc",
                        Template = "6628f5244c3764ecf474f23c",
                        ParentId = "3a53768e6ebfca707bf25f2e",
                        SlotId = "mod_magazine",
                    },
                    new Item
                    {
                        Id = "f758841d14b7b1a412473b4f",
                        Template = "6628f534d238d16e71d27601",
                        ParentId = "3a53768e6ebfca707bf25f2e",
                        SlotId = "mod_reciever",
                    },
                    new Item
                    {
                        Id = "a7eb783903eddf54d0bea5e3",
                        Template = "5d120a10d7ad1a4e1026ba85",
                        ParentId = "3a53768e6ebfca707bf25f2e",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "41d3c85112553af8a7cf67a4",
                        Template = "5b2240bf5acfc40dc528af69",
                        ParentId = "3a53768e6ebfca707bf25f2e",
                        SlotId = "mod_charge",
                    },
                    new Item
                    {
                        Id = "03fc17f51b25fe0c508d3121",
                        Template = "584924ec24597768f12ae244",
                        ParentId = "f758841d14b7b1a412473b4f",
                        SlotId = "mod_scope",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "e73565a04e2aa80d36c020b4",
                        Template = "63d3ce0446bd475bcb50f55f",
                        ParentId = "f758841d14b7b1a412473b4f",
                        SlotId = "mod_barrel",
                    },
                    new Item
                    {
                        Id = "380c890975fc3fc0f7dbace8",
                        Template = "619b5db699fb192e7430664f",
                        ParentId = "f758841d14b7b1a412473b4f",
                        SlotId = "mod_handguard",
                    },
                    new Item
                    {
                        Id = "5ad2e9e4839ea36238268690",
                        Template = "5ba26b17d4351e00367f9bdd",
                        ParentId = "f758841d14b7b1a412473b4f",
                        SlotId = "mod_sight_rear",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "1c90fe1f6e52b6dd50c2d418",
                        Template = "5d120a28d7ad1a1c8962e295",
                        ParentId = "a7eb783903eddf54d0bea5e3",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "c9143e70a0174769677426e9",
                        Template = "6405ff6bd4578826ec3e377a",
                        ParentId = "e73565a04e2aa80d36c020b4",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "e62bab6a3d46c698556cff8d",
                        Template = "56eabcd4d2720b66698b4574",
                        ParentId = "e73565a04e2aa80d36c020b4",
                        SlotId = "mod_gas_block",
                    },
                    new Item
                    {
                        Id = "677c018c049095a184d37200",
                        Template = "5b7be47f5acfc400170e2dd2",
                        ParentId = "380c890975fc3fc0f7dbace8",
                        SlotId = "mod_mount_000",
                    },
                    new Item
                    {
                        Id = "083c7f61401f544aff586505",
                        Template = "57cffb66245977632f391a99",
                        ParentId = "380c890975fc3fc0f7dbace8",
                        SlotId = "mod_foregrip",
                    },
                    new Item
                    {
                        Id = "0d1c57608f0a9ee4488a3032",
                        Template = "5ba26b01d4351e0085325a51",
                        ParentId = "380c890975fc3fc0f7dbace8",
                        SlotId = "mod_sight_front",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "2aaf65a742bd972e2b74bdbf",
                        Template = "6272370ee4013c5d7e31f418",
                        ParentId = "677c018c049095a184d37200",
                        SlotId = "mod_tactical",
                        Upd = new Upd
                        {
                            Light = new UpdLight { IsActive = true, SelectedMode = 0 },
                        },
                    },
                },
            }
        },
        {
            "67801a95b7414980a764239d",
            new Preset
            {
                Id = "67801a95b7414980a764239d",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "MCM4 762",
                Parent = "16d46f3248d7972e2d47b0fa",
                Encyclopedia = "6628f50c78434b5effe5ced1",
                Items = new List<Item>
                {
                    new Item
                    {
                        Id = "16d46f3248d7972e2d47b0fa",
                        Template = "6628f50c78434b5effe5ced1",
                        Upd = new Upd
                        {
                            Repairable = new UpdRepairable { MaxDurability = 100, Durability = 100 },
                            FireMode = new UpdFireMode { FireMode = "single" },
                        },
                    },
                    new Item
                    {
                        Id = "7e75ecc3f3e5e77f49061a1c",
                        Template = "5d025cc1d7ad1a53845279ef",
                        ParentId = "16d46f3248d7972e2d47b0fa",
                        SlotId = "mod_pistol_grip",
                    },
                    new Item
                    {
                        Id = "534bf3501426592782d3b5ef",
                        Template = "6628f51e54b4f59a7ade4ef9",
                        ParentId = "16d46f3248d7972e2d47b0fa",
                        SlotId = "mod_magazine",
                    },
                    new Item
                    {
                        Id = "c116bc96b7294f5901b5f6cf",
                        Template = "6628f531ae7642ade7384366",
                        ParentId = "16d46f3248d7972e2d47b0fa",
                        SlotId = "mod_reciever",
                    },
                    new Item
                    {
                        Id = "37829dfd35a196e85ebe870c",
                        Template = "5b0800175acfc400153aebd4",
                        ParentId = "16d46f3248d7972e2d47b0fa",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "8db6d11f8c55ca7444571c9e",
                        Template = "651bf5617b3b552ef6712cb7",
                        ParentId = "16d46f3248d7972e2d47b0fa",
                        SlotId = "mod_charge",
                    },
                    new Item
                    {
                        Id = "93a97deddcbf6d0e87c0a3ce",
                        Template = "5a37ca54c4a282000d72296a",
                        ParentId = "c116bc96b7294f5901b5f6cf",
                        SlotId = "mod_scope",
                    },
                    new Item
                    {
                        Id = "42644d20ea11bf887586419b",
                        Template = "5d440b9fa4b93601354d480c",
                        ParentId = "c116bc96b7294f5901b5f6cf",
                        SlotId = "mod_barrel",
                    },
                    new Item
                    {
                        Id = "5f4342368d5d328a4bcd1b87",
                        Template = "5b2cfa535acfc432ff4db7a0",
                        ParentId = "c116bc96b7294f5901b5f6cf",
                        SlotId = "mod_handguard",
                    },
                    new Item
                    {
                        Id = "689df70a8f51b32b8e9b6bef",
                        Template = "5c1780312e221602b66cc189",
                        ParentId = "c116bc96b7294f5901b5f6cf",
                        SlotId = "mod_sight_rear",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "bb171b58830e0d780c91922d",
                        Template = "5b3b99475acfc432ff4dcbee",
                        ParentId = "93a97deddcbf6d0e87c0a3ce",
                        SlotId = "mod_scope_000",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "ec951bfbe8b6db71529dea79",
                        Template = "58d268fc86f774111273f8c2",
                        ParentId = "93a97deddcbf6d0e87c0a3ce",
                        SlotId = "mod_scope_001",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "9981433d49d49a44594599dd",
                        Template = "5ea172e498dacb342978818e",
                        ParentId = "42644d20ea11bf887586419b",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "e1ea502cb06c7deddabffcc1",
                        Template = "5d00ec68d7ad1a04a067e5be",
                        ParentId = "42644d20ea11bf887586419b",
                        SlotId = "mod_gas_block",
                    },
                    new Item
                    {
                        Id = "bda7f30aa4f7c574c7be4a93",
                        Template = "5b30bc285acfc47a8608615d",
                        ParentId = "5f4342368d5d328a4bcd1b87",
                        SlotId = "mod_mount_000",
                    },
                    new Item
                    {
                        Id = "981288b5268cfa9956160f2b",
                        Template = "55d614004bdc2d86028b4568",
                        ParentId = "9981433d49d49a44594599dd",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "911000d3fa24576716b9ea27",
                        Template = "558032614bdc2de7118b4585",
                        ParentId = "bda7f30aa4f7c574c7be4a93",
                        SlotId = "mod_foregrip",
                    },
                    new Item
                    {
                        Id = "cb17309b8c076da435eaebce",
                        Template = "544909bb4bdc2d6f028b4577",
                        ParentId = "bda7f30aa4f7c574c7be4a93",
                        SlotId = "mod_tactical",
                        Upd = new Upd
                        {
                            Light = new UpdLight { IsActive = true, SelectedMode = 0 },
                        },
                    },
                },
            }
        },
        {
            "67801a8f9febc36b38d1b220",
            new Preset
            {
                Id = "67801a8f9febc36b38d1b220",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "MCM4 545",
                Parent = "f7bccda9c3b50d02b134d4d8",
                Encyclopedia = "6628f50c78434b5effe5ced1",
                Items = new List<Item>
                {
                    new Item
                    {
                        Id = "f7bccda9c3b50d02b134d4d8",
                        Template = "6628f50c78434b5effe5ced1",
                        Upd = new Upd
                        {
                            Repairable = new UpdRepairable { MaxDurability = 100, Durability = 100 },
                            FireMode = new UpdFireMode { FireMode = "single" },
                        },
                    },
                    new Item
                    {
                        Id = "35de17204da89a0b6de66e3f",
                        Template = "5a33e75ac4a2826c6e06d759",
                        ParentId = "f7bccda9c3b50d02b134d4d8",
                        SlotId = "mod_pistol_grip",
                    },
                    new Item
                    {
                        Id = "cfa24a1340f1cfc3ddb6ad1a",
                        Template = "6628f51c8df290deed8983e9",
                        ParentId = "f7bccda9c3b50d02b134d4d8",
                        SlotId = "mod_magazine",
                    },
                    new Item
                    {
                        Id = "f64553f1c244fa19da778410",
                        Template = "6628f52f246a7106c3c3dd22",
                        ParentId = "f7bccda9c3b50d02b134d4d8",
                        SlotId = "mod_reciever",
                    },
                    new Item
                    {
                        Id = "54f23efb6abdd3adc0323eec",
                        Template = "5c793fb92e221644f31bfb64",
                        ParentId = "f7bccda9c3b50d02b134d4d8",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "0274df68c8d4df1686f1ff1a",
                        Template = "5f633ff5c444ce7e3c30a006",
                        ParentId = "f7bccda9c3b50d02b134d4d8",
                        SlotId = "mod_charge",
                    },
                    new Item
                    {
                        Id = "8e274cca86bc6dc6aa1741ac",
                        Template = "5c07dd120db834001c39092d",
                        ParentId = "f64553f1c244fa19da778410",
                        SlotId = "mod_scope",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "fd8dbc6b8dff223bc6646ff0",
                        Template = "63d3d44a2a49307baf09386d",
                        ParentId = "f64553f1c244fa19da778410",
                        SlotId = "mod_barrel",
                    },
                    new Item
                    {
                        Id = "59bac38cfdb1dcb28cfd3c17",
                        Template = "619b5db699fb192e7430664f",
                        ParentId = "f64553f1c244fa19da778410",
                        SlotId = "mod_handguard",
                    },
                    new Item
                    {
                        Id = "8488cba3765484b78863ef9f",
                        Template = "5ba26b17d4351e00367f9bdd",
                        ParentId = "f64553f1c244fa19da778410",
                        SlotId = "mod_sight_rear",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "21f290e41b1518ad53c9664d",
                        Template = "5a9fbb84a2750c00137fa685",
                        ParentId = "fd8dbc6b8dff223bc6646ff0",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "0e342cf53e906295a1492d43",
                        Template = "56eabcd4d2720b66698b4574",
                        ParentId = "fd8dbc6b8dff223bc6646ff0",
                        SlotId = "mod_gas_block",
                    },
                    new Item
                    {
                        Id = "f9cf53d13d657908bcf6c3ea",
                        Template = "5b7be4895acfc400170e2dd5",
                        ParentId = "59bac38cfdb1dcb28cfd3c17",
                        SlotId = "mod_foregrip",
                    },
                    new Item
                    {
                        Id = "b94107c91491bf064ecef74d",
                        Template = "5ba26b01d4351e0085325a51",
                        ParentId = "59bac38cfdb1dcb28cfd3c17",
                        SlotId = "mod_sight_front",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "abb954a1250a58a28b3d8f80",
                        Template = "5c791e872e2216001219c40a",
                        ParentId = "f9cf53d13d657908bcf6c3ea",
                        SlotId = "mod_foregrip",
                    },
                },
            }
        },
        {
            "67801a8a0db19fcdea8ec517",
            new Preset
            {
                Id = "67801a8a0db19fcdea8ec517",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "MCM4 939",
                Parent = "e6eab374e5f2929849b897f8",
                Encyclopedia = "6628f50c78434b5effe5ced1",
                Items = new List<Item>
                {
                    new Item
                    {
                        Id = "e6eab374e5f2929849b897f8",
                        Template = "6628f50c78434b5effe5ced1",
                        Upd = new Upd
                        {
                            Repairable = new UpdRepairable { MaxDurability = 100, Durability = 100 },
                            FireMode = new UpdFireMode { FireMode = "single" },
                        },
                    },
                    new Item
                    {
                        Id = "f6b7b6f5353f59aaf41ff6ed",
                        Template = "59db3a1d86f77429e05b4e92",
                        ParentId = "e6eab374e5f2929849b897f8",
                        SlotId = "mod_pistol_grip",
                    },
                    new Item
                    {
                        Id = "b74ab33383619030700b83dd",
                        Template = "6628f5191c445ab1b8b8cdf5",
                        ParentId = "e6eab374e5f2929849b897f8",
                        SlotId = "mod_magazine",
                    },
                    new Item
                    {
                        Id = "92bcd1abc4eb5c587a47e1b6",
                        Template = "6628f52b821b61722b245c10",
                        ParentId = "e6eab374e5f2929849b897f8",
                        SlotId = "mod_reciever",
                    },
                    new Item
                    {
                        Id = "8b9395cc1391b811b4b0d695",
                        Template = "627254cc9c563e6e442c398f",
                        ParentId = "e6eab374e5f2929849b897f8",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "ed71a8544ad06f442467afe1",
                        Template = "5d44334ba4b9362b346d1948",
                        ParentId = "e6eab374e5f2929849b897f8",
                        SlotId = "mod_charge",
                    },
                    new Item
                    {
                        Id = "cdf0f8d8423454ae3315213a",
                        Template = "544a3a774bdc2d3a388b4567",
                        ParentId = "92bcd1abc4eb5c587a47e1b6",
                        SlotId = "mod_scope",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "12002669a65b242963ef3509",
                        Template = "63d3ce0446bd475bcb50f55f",
                        ParentId = "92bcd1abc4eb5c587a47e1b6",
                        SlotId = "mod_barrel",
                    },
                    new Item
                    {
                        Id = "b4bdb1e7b7528820b91e6fc5",
                        Template = "5c78f2612e221600114c9f0d",
                        ParentId = "92bcd1abc4eb5c587a47e1b6",
                        SlotId = "mod_handguard",
                    },
                    new Item
                    {
                        Id = "a6d67601b7998f6c8746652c",
                        Template = "5fb6564947ce63734e3fa1da",
                        ParentId = "92bcd1abc4eb5c587a47e1b6",
                        SlotId = "mod_sight_rear",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "39e62538ce4785caddccde39",
                        Template = "58d268fc86f774111273f8c2",
                        ParentId = "cdf0f8d8423454ae3315213a",
                        SlotId = "mod_scope",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "c0792804acaee0a58dcc55f3",
                        Template = "609269c3b0e443224b421cc1",
                        ParentId = "12002669a65b242963ef3509",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "0eb41a46ad2f1be1b60da571",
                        Template = "56eabcd4d2720b66698b4574",
                        ParentId = "12002669a65b242963ef3509",
                        SlotId = "mod_gas_block",
                    },
                    new Item
                    {
                        Id = "678bab8b6349599aca86fb99",
                        Template = "651a8bf3a8520e48047bf708",
                        ParentId = "b4bdb1e7b7528820b91e6fc5",
                        SlotId = "mod_foregrip",
                    },
                    new Item
                    {
                        Id = "dea4d2dc893fca0ce06817de",
                        Template = "5fb6567747ce63734e3fa1dc",
                        ParentId = "b4bdb1e7b7528820b91e6fc5",
                        SlotId = "mod_sight_front",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "478e2bcb1f026d13f693f903",
                        Template = "5a7b483fe899ef0016170d15",
                        ParentId = "b4bdb1e7b7528820b91e6fc5",
                        SlotId = "mod_tactical_000",
                        Upd = new Upd
                        {
                            Light = new UpdLight { IsActive = true, SelectedMode = 0 },
                        },
                    },
                    new Item
                    {
                        Id = "bddc9d0b1c39ded9c913044a",
                        Template = "60926df0132d4d12c81fd9df",
                        ParentId = "c0792804acaee0a58dcc55f3",
                        SlotId = "mod_muzzle",
                    },
                },
            }
        },
        {
            "67801a85ba702f398820f263",
            new Preset
            {
                Id = "67801a85ba702f398820f263",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "MCM4 300",
                Parent = "99dcd3e0129ff18aaad8fa01",
                Encyclopedia = "6628f50c78434b5effe5ced1",
                Items = new List<Item>
                {
                    new Item
                    {
                        Id = "99dcd3e0129ff18aaad8fa01",
                        Template = "6628f50c78434b5effe5ced1",
                        Upd = new Upd
                        {
                            Repairable = new UpdRepairable { MaxDurability = 100, Durability = 100 },
                            FireMode = new UpdFireMode { FireMode = "single" },
                        },
                    },
                    new Item
                    {
                        Id = "024a7250e11da1da56e2f9ee",
                        Template = "63f5feead259b42f0b4d6d0f",
                        ParentId = "99dcd3e0129ff18aaad8fa01",
                        SlotId = "mod_pistol_grip",
                    },
                    new Item
                    {
                        Id = "337153653fe895a12837eb4c",
                        Template = "6628f511ea0ab10dd0c5e2d4",
                        ParentId = "99dcd3e0129ff18aaad8fa01",
                        SlotId = "mod_magazine",
                    },
                    new Item
                    {
                        Id = "9bed8756af0eb89e39e47d83",
                        Template = "6628f529b9886fa300b47de7",
                        ParentId = "99dcd3e0129ff18aaad8fa01",
                        SlotId = "mod_reciever",
                    },
                    new Item
                    {
                        Id = "c5815a97947d85d537638980",
                        Template = "5d120a10d7ad1a4e1026ba85",
                        ParentId = "99dcd3e0129ff18aaad8fa01",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "a76ec68961907effea90cac2",
                        Template = "56ea7165d2720b6e518b4583",
                        ParentId = "99dcd3e0129ff18aaad8fa01",
                        SlotId = "mod_charge",
                    },
                    new Item
                    {
                        Id = "b429f2710c6824c0037c130f",
                        Template = "5d2dc3e548f035404a1a4798",
                        ParentId = "9bed8756af0eb89e39e47d83",
                        SlotId = "mod_scope",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "4440704bf4d56f5617618224",
                        Template = "5d440b93a4b9364276578d4b",
                        ParentId = "9bed8756af0eb89e39e47d83",
                        SlotId = "mod_barrel",
                    },
                    new Item
                    {
                        Id = "e9e6d462067e29a0cc6c8a04",
                        Template = "5d4405f0a4b9361e6a4e6bd9",
                        ParentId = "9bed8756af0eb89e39e47d83",
                        SlotId = "mod_handguard",
                    },
                    new Item
                    {
                        Id = "5bed464f2ea778cc3264aebb",
                        Template = "5bb20e49d4351e3bac1212de",
                        ParentId = "9bed8756af0eb89e39e47d83",
                        SlotId = "mod_sight_rear",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "89b2f4cc5f880485ca32a241",
                        Template = "5d120a28d7ad1a1c8962e295",
                        ParentId = "c5815a97947d85d537638980",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "ac29fd49f0b192497019a406",
                        Template = "6386120cd6baa055ad1e201c",
                        ParentId = "4440704bf4d56f5617618224",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "34b7a972bdade9ef6e0e5941",
                        Template = "56eabcd4d2720b66698b4574",
                        ParentId = "4440704bf4d56f5617618224",
                        SlotId = "mod_gas_block",
                    },
                    new Item
                    {
                        Id = "9f9218e4fa9aad08d0b909ed",
                        Template = "5649a2464bdc2d91118b45a8",
                        ParentId = "e9e6d462067e29a0cc6c8a04",
                        SlotId = "mod_scope",
                    },
                    new Item
                    {
                        Id = "2844ca3724b1f0bc5133e36b",
                        Template = "6269545d0e57f218e4548ca2",
                        ParentId = "e9e6d462067e29a0cc6c8a04",
                        SlotId = "mod_mount_002",
                    },
                    new Item
                    {
                        Id = "58f81045bc80c06268c71210",
                        Template = "5c17804b2e2216152006c02f",
                        ParentId = "e9e6d462067e29a0cc6c8a04",
                        SlotId = "mod_sight_front",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                    new Item
                    {
                        Id = "f1f4d5de9f21b14faff84cd5",
                        Template = "5b7be4895acfc400170e2dd5",
                        ParentId = "e9e6d462067e29a0cc6c8a04",
                        SlotId = "mod_foregrip",
                    },
                    new Item
                    {
                        Id = "20feda8db8eb6e3310949f13",
                        Template = "638612b607dfed1ccb7206ba",
                        ParentId = "ac29fd49f0b192497019a406",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "4b8d764dd12527697de363bb",
                        Template = "5a33b2c9c4a282000c5a9511",
                        ParentId = "9f9218e4fa9aad08d0b909ed",
                        SlotId = "mod_scope",
                    },
                    new Item
                    {
                        Id = "b167d5ab0c0da606643e7a7f",
                        Template = "5b07dd285acfc4001754240d",
                        ParentId = "2844ca3724b1f0bc5133e36b",
                        SlotId = "mod_tactical",
                        Upd = new Upd
                        {
                            Light = new UpdLight { IsActive = true, SelectedMode = 0 },
                        },
                    },
                    new Item
                    {
                        Id = "ac48b7f10d304edff2b7f4c1",
                        Template = "5b057b4f5acfc4771e1bd3e9",
                        ParentId = "f1f4d5de9f21b14faff84cd5",
                        SlotId = "mod_foregrip",
                    },
                    new Item
                    {
                        Id = "5ae8da6f7f8a43a03d01f691",
                        Template = "5a32aa8bc4a2826c6e06d737",
                        ParentId = "4b8d764dd12527697de363bb",
                        SlotId = "mod_scope",
                        Upd = new Upd
                        {
                            Sight = new UpdSight
                            {
                                ScopesCurrentCalibPointIndexes = [0],
                                ScopesSelectedModes = [0],
                                SelectedScope = 0,
                            },
                        },
                    },
                },
            }
        },
        {
            "67801a81f1e1708566957a9c",
            new Preset
            {
                Id = "67801a81f1e1708566957a9c",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "Vest Rhino Standard",
                Parent = "6621b36d9abe55b76c03eecf",
                Encyclopedia = "6621b1f22310f5ff09476360",
                Items = new List<Item>
                {
                    new Item { Id = "6621b36d9abe55b76c03eecf", Template = "6621b1f22310f5ff09476360" },
                    new Item
                    {
                        Id = "6576e5ce424f2c87d30c75d2",
                        Template = "6570e5100b57c03ec90b970a",
                        ParentId = "6621b36d9abe55b76c03eecf",
                        SlotId = "Soft_armor_front",
                    },
                    new Item
                    {
                        Id = "6576e5ce424f2c87d30c75d3",
                        Template = "6570e479a6560e4ee50c2b02",
                        ParentId = "6621b36d9abe55b76c03eecf",
                        SlotId = "Soft_armor_back",
                    },
                    new Item
                    {
                        Id = "6576e5ce424f2c87d30c75d4",
                        Template = "6570e5674cc0d2ab1e05edbb",
                        ParentId = "6621b36d9abe55b76c03eecf",
                        SlotId = "Soft_armor_left",
                    },
                    new Item
                    {
                        Id = "6576e5ce424f2c87d30c75d5",
                        Template = "6570e59b0b57c03ec90b970e",
                        ParentId = "6621b36d9abe55b76c03eecf",
                        SlotId = "soft_armor_right",
                    },
                    new Item
                    {
                        Id = "6576e5ce424f2c87d30c75d6",
                        Template = "656f9fa0498d1b7e3e071d98",
                        ParentId = "6621b36d9abe55b76c03eecf",
                        SlotId = "Front_plate",
                    },
                    new Item
                    {
                        Id = "6576e5ce424f2c87d30c75d7",
                        Template = "656f9fa0498d1b7e3e071d98",
                        ParentId = "6621b36d9abe55b76c03eecf",
                        SlotId = "Back_plate",
                    },
                },
            }
        },
        {
            "67801a7d83ffd332b33c8471",
            new Preset
            {
                Id = "67801a7d83ffd332b33c8471",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "Body armor Banshee Standard",
                Parent = "6621b3ac4404ba2495f3247a",
                Encyclopedia = "6621b1e0bd80eeb4de0c9ed8",
                Items = new List<Item>
                {
                    new Item { Id = "6621b3ac4404ba2495f3247a", Template = "6621b1e0bd80eeb4de0c9ed8" },
                    new Item
                    {
                        Id = "6576ec5406a990316201e800",
                        Template = "656faf0ca0dce000a2020f77",
                        ParentId = "6621b3ac4404ba2495f3247a",
                        SlotId = "front_plate",
                    },
                    new Item
                    {
                        Id = "6576ec5406a990316201e801",
                        Template = "656faf0ca0dce000a2020f77",
                        ParentId = "6621b3ac4404ba2495f3247a",
                        SlotId = "back_plate",
                    },
                },
            }
        },
        {
            "67801a787a2d205b6b06ff72",
            new Preset
            {
                Id = "67801a787a2d205b6b06ff72",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "Vest Osprey Armless Standard",
                Parent = "6621b3e205636445378bfd6e",
                Encyclopedia = "6621b274485df00e750df4f2",
                Items = new List<Item>
                {
                    new Item { Id = "6621b3e205636445378bfd6e", Template = "6621b274485df00e750df4f2" },
                    new Item
                    {
                        Id = "6621b3f15aa95a621a695bd4",
                        Template = "6570e5100b57c03ec90b970a",
                        ParentId = "6621b3e205636445378bfd6e",
                        SlotId = "Soft_armor_front",
                    },
                    new Item
                    {
                        Id = "6621b3f62abd25f54483bf5d",
                        Template = "6570e479a6560e4ee50c2b02",
                        ParentId = "6621b3e205636445378bfd6e",
                        SlotId = "Soft_armor_back",
                    },
                    new Item
                    {
                        Id = "6621b3fb603ae9c8215084bf",
                        Template = "6570e5674cc0d2ab1e05edbb",
                        ParentId = "6621b3e205636445378bfd6e",
                        SlotId = "Soft_armor_left",
                    },
                    new Item
                    {
                        Id = "6621b401d7c51008269dcd24",
                        Template = "6570e59b0b57c03ec90b970e",
                        ParentId = "6621b3e205636445378bfd6e",
                        SlotId = "soft_armor_right",
                    },
                    new Item
                    {
                        Id = "6621b40624047caf313f2a3e",
                        Template = "656f9fa0498d1b7e3e071d98",
                        ParentId = "6621b3e205636445378bfd6e",
                        SlotId = "Front_plate",
                    },
                    new Item
                    {
                        Id = "6621b40ac1720868593eb226",
                        Template = "656f9fa0498d1b7e3e071d98",
                        ParentId = "6621b3e205636445378bfd6e",
                        SlotId = "Back_plate",
                    },
                },
            }
        },
        {
            "67801a74b882fe02c463262f",
            new Preset
            {
                Id = "67801a74b882fe02c463262f",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "Fear",
                Parent = "66282d1b014e668fc57bedd8",
                Encyclopedia = "6628f8813e3fe94f5f035010",
                Items = new List<Item>
                {
                    new Item { Id = "66282d1b014e668fc57bedd8", Template = "6628f8813e3fe94f5f035010" },
                    new Item
                    {
                        Id = "66282d25b6f41a18c86ee83c",
                        Template = "59db3a1d86f77429e05b4e92",
                        ParentId = "66282d1b014e668fc57bedd8",
                        SlotId = "mod_pistol_grip",
                    },
                    new Item
                    {
                        Id = "66282d2f6030ff262a2f0f6d",
                        Template = "6628f88aaa160dcd09a782e7",
                        ParentId = "66282d1b014e668fc57bedd8",
                        SlotId = "mod_magazine",
                    },
                    new Item
                    {
                        Id = "66282d36302b16cb1df5d6cf",
                        Template = "6628f885b95f6ae9d977162f",
                        ParentId = "66282d1b014e668fc57bedd8",
                        SlotId = "mod_reciever",
                    },
                    new Item
                    {
                        Id = "66282d3be4dc9f789c9be8f7",
                        Template = "5c5db6f82e2216003a0fe914",
                        ParentId = "66282d1b014e668fc57bedd8",
                        SlotId = "mod_stock_000",
                    },
                    new Item
                    {
                        Id = "66282d42c4beba3d571e95d3",
                        Template = "6529109524cbe3c74a05e5b7",
                        ParentId = "66282d1b014e668fc57bedd8",
                        SlotId = "mod_charge",
                    },
                    new Item
                    {
                        Id = "66282d48b1aa1769e9276880",
                        Template = "558022b54bdc2dac148b458d",
                        ParentId = "66282d36302b16cb1df5d6cf",
                        SlotId = "mod_scope",
                    },
                    new Item
                    {
                        Id = "66282d4c1357c98e30161fe0",
                        Template = "652910565ae2ae97b80fdf35",
                        ParentId = "66282d36302b16cb1df5d6cf",
                        SlotId = "mod_barrel",
                    },
                    new Item
                    {
                        Id = "66282d53c8dd7430e211b5f5",
                        Template = "652910ef50dc782999054b97",
                        ParentId = "66282d36302b16cb1df5d6cf",
                        SlotId = "mod_handguard",
                    },
                    new Item
                    {
                        Id = "66282d5df98894aecc5d687c",
                        Template = "5c1780312e221602b66cc189",
                        ParentId = "66282d36302b16cb1df5d6cf",
                        SlotId = "mod_sight_rear",
                    },
                    new Item
                    {
                        Id = "66282d60ec48a9f71de99998",
                        Template = "6529113b5ae2ae97b80fdf39",
                        ParentId = "66282d4c1357c98e30161fe0",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "66282d66ec4d55e7c5660f54",
                        Template = "652910bc24cbe3c74a05e5b9",
                        ParentId = "66282d4c1357c98e30161fe0",
                        SlotId = "mod_gas_block",
                    },
                    new Item
                    {
                        Id = "66282d683dc78e7f777fbea8",
                        Template = "5c17804b2e2216152006c02f",
                        ParentId = "66282d53c8dd7430e211b5f5",
                        SlotId = "mod_sight_front",
                    },
                    new Item
                    {
                        Id = "66282d6ba4f053a46af91386",
                        Template = "5b7be4895acfc400170e2dd5",
                        ParentId = "66282d53c8dd7430e211b5f5",
                        SlotId = "mod_foregrip",
                    },
                    new Item
                    {
                        Id = "66282d717d155ce6f546d366",
                        Template = "652911e650dc782999054b9d",
                        ParentId = "66282d60ec48a9f71de99998",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "66282d7637bdfd7e1954be49",
                        Template = "655df24fdf80b12750626d0a",
                        ParentId = "66282d6ba4f053a46af91386",
                        SlotId = "mod_foregrip",
                    },
                },
            }
        },
        {
            "67801a6df18b3307ff78608c",
            new Preset
            {
                Id = "67801a6df18b3307ff78608c",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "Dread",
                Parent = "66282dd1143c620e3be2729f",
                Encyclopedia = "6628f96fd59ab54dedb55801",
                Items = new List<Item>
                {
                    new Item { Id = "66282dd1143c620e3be2729f", Template = "6628f96fd59ab54dedb55801" },
                    new Item
                    {
                        Id = "66282dd6e550612bc9ef07f4",
                        Template = "6628f973b21453a8afc0db68",
                        ParentId = "66282dd1143c620e3be2729f",
                        SlotId = "mod_magazine",
                    },
                    new Item
                    {
                        Id = "66282ddbc21944f42f764069",
                        Template = "5d25d0ac8abbc3054f3e61f7",
                        ParentId = "66282dd1143c620e3be2729f",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "66282de03beaf4499b270dbd",
                        Template = "5d2703038abbc3105103d94c",
                        ParentId = "66282dd1143c620e3be2729f",
                        SlotId = "mod_barrel",
                    },
                    new Item
                    {
                        Id = "66282de64a67a013ab7f85e5",
                        Template = "5bfebc5e0db834001a6694e5",
                        ParentId = "66282dd1143c620e3be2729f",
                        SlotId = "mod_mount",
                    },
                    new Item
                    {
                        Id = "66282df07457cb23c1f6f4d3",
                        Template = "5a9d6d00a2750c5c985b5305",
                        ParentId = "66282ddbc21944f42f764069",
                        SlotId = "mod_mount_000",
                    },
                    new Item
                    {
                        Id = "66282df478f3fba092ba3d9e",
                        Template = "6130c43c67085e45ef1405a1",
                        ParentId = "66282de03beaf4499b270dbd",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "66282df8e362c6a8f0d14105",
                        Template = "5b3b99475acfc432ff4dcbee",
                        ParentId = "66282de64a67a013ab7f85e5",
                        SlotId = "mod_scope",
                    },
                    new Item
                    {
                        Id = "66282dfc1b487f5832e0d102",
                        Template = "544909bb4bdc2d6f028b4577",
                        ParentId = "66282df07457cb23c1f6f4d3",
                        SlotId = "mod_tactical",
                    },
                    new Item
                    {
                        Id = "66282dff45f1a1ab1ced592c",
                        Template = "5dfa3d2b0dee1b22f862eade",
                        ParentId = "66282df478f3fba092ba3d9e",
                        SlotId = "mod_muzzle",
                    },
                },
            }
        },
        {
            "67801a682af46237eff2e4c2",
            new Preset
            {
                Id = "67801a682af46237eff2e4c2",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "Nightmare",
                Parent = "66282e13a3168a5a05daee2c",
                Encyclopedia = "6628f76df1a913e3afc16360",
                Items = new List<Item>
                {
                    new Item { Id = "66282e13a3168a5a05daee2c", Template = "6628f76df1a913e3afc16360" },
                    new Item
                    {
                        Id = "66282e17c9b56ef1fe2827a9",
                        Template = "5a6b5e468dc32e001207faf5",
                        ParentId = "66282e13a3168a5a05daee2c",
                        SlotId = "mod_barrel",
                    },
                    new Item
                    {
                        Id = "66282e1cc6e177cc8ebb4090",
                        Template = "5a7b4960e899ef197b331a2d",
                        ParentId = "66282e13a3168a5a05daee2c",
                        SlotId = "mod_pistol_grip",
                    },
                    new Item
                    {
                        Id = "66282e20a387fac998ac257d",
                        Template = "6628f7705124be8295a0bdd9",
                        ParentId = "66282e13a3168a5a05daee2c",
                        SlotId = "mod_reciever",
                    },
                    new Item
                    {
                        Id = "66282e25589749c6252175c6",
                        Template = "6628f7786fbce2d268af4913",
                        ParentId = "66282e13a3168a5a05daee2c",
                        SlotId = "mod_magazine",
                    },
                    new Item
                    {
                        Id = "66282e29277efc692e12b88e",
                        Template = "5a7ad4af51dfba0013379717",
                        ParentId = "66282e13a3168a5a05daee2c",
                        SlotId = "mod_tactical",
                    },
                    new Item
                    {
                        Id = "66282e2d66c78a9caf8ab399",
                        Template = "5d1c702ad7ad1a632267f429",
                        ParentId = "66282e13a3168a5a05daee2c",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "66282e318cc440c0f5822126",
                        Template = "5a32a064c4a28200741e22de",
                        ParentId = "66282e17c9b56ef1fe2827a9",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "66282e362c07e620347c4de5",
                        Template = "5a7d9122159bd4001438dbf4",
                        ParentId = "66282e20a387fac998ac257d",
                        SlotId = "mod_sight_rear",
                    },
                    new Item
                    {
                        Id = "66282e39267a68d22ddd5988",
                        Template = "5a7d90eb159bd400165484f1",
                        ParentId = "66282e20a387fac998ac257d",
                        SlotId = "mod_sight_front",
                    },
                    new Item
                    {
                        Id = "66282e3ce2764dc2bcd81c6b",
                        Template = "616554fe50224f204c1da2aa",
                        ParentId = "66282e29277efc692e12b88e",
                        SlotId = "mod_scope",
                    },
                    new Item
                    {
                        Id = "66282e40e4cbe3c88c3b91f8",
                        Template = "61657230d92c473c770213d7",
                        ParentId = "66282e3ce2764dc2bcd81c6b",
                        SlotId = "mod_scope",
                    },
                },
            }
        },
        {
            "67801a631a22f8da19d6cffc",
            new Preset
            {
                Id = "67801a631a22f8da19d6cffc",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "AUG 762",
                Parent = "c54359f7d804a614642382c0",
                Encyclopedia = "6628f2e31084cf4d62b2b40b",
                Items = new List<Item>
                {
                    new Item { Id = "c54359f7d804a614642382c0", Template = "6628f2e31084cf4d62b2b40b" },
                    new Item
                    {
                        Id = "04a053de7b30ac5c2634e638",
                        Template = "6628f33d585102d8bc5d75db",
                        ParentId = "c54359f7d804a614642382c0",
                        SlotId = "mod_magazine",
                    },
                    new Item
                    {
                        Id = "d3645623a1b68e602608cc24",
                        Template = "62ebbc53e3c1e1ec7c02c44f",
                        ParentId = "c54359f7d804a614642382c0",
                        SlotId = "mod_charge",
                    },
                    new Item
                    {
                        Id = "7a89fe1c63cfd07dcce2b69c",
                        Template = "62e7c72df68e7a0676050c77",
                        ParentId = "c54359f7d804a614642382c0",
                        SlotId = "mod_reciever",
                    },
                    new Item
                    {
                        Id = "34a60b6dc30c34a20c8d2864",
                        Template = "62e7c7f3c34ea971710c32fc",
                        ParentId = "7a89fe1c63cfd07dcce2b69c",
                        SlotId = "mod_barrel",
                    },
                    new Item
                    {
                        Id = "065e966be13638b545da239b",
                        Template = "62e7c8f91cd3fde4d503d690",
                        ParentId = "7a89fe1c63cfd07dcce2b69c",
                        SlotId = "mod_mount",
                    },
                    new Item
                    {
                        Id = "42f300565e0d82bf8bde86ca",
                        Template = "644a3df63b0b6f03e101e065",
                        ParentId = "7a89fe1c63cfd07dcce2b69c",
                        SlotId = "mod_tactical",
                    },
                    new Item
                    {
                        Id = "ebc104d4e4e293097416e7b0",
                        Template = "630f28f0cadb1fe05e06f004",
                        ParentId = "34a60b6dc30c34a20c8d2864",
                        SlotId = "mod_muzzle_000",
                    },
                    new Item
                    {
                        Id = "0f094eb67cb4e7b4a945636c",
                        Template = "630f2982cdb9e392db0cbcc7",
                        ParentId = "34a60b6dc30c34a20c8d2864",
                        SlotId = "mod_muzzle_001",
                    },
                    new Item
                    {
                        Id = "72c912b21b773044670f666c",
                        Template = "634e61b0767cb15c4601a877",
                        ParentId = "34a60b6dc30c34a20c8d2864",
                        SlotId = "mod_foregrip",
                    },
                    new Item
                    {
                        Id = "8314b95662e847cd1cda4ebb",
                        Template = "584924ec24597768f12ae244",
                        ParentId = "065e966be13638b545da239b",
                        SlotId = "mod_scope",
                    },
                    new Item
                    {
                        Id = "f32b5938cc8ea69adbb06a12",
                        Template = "5c17804b2e2216152006c02f",
                        ParentId = "065e966be13638b545da239b",
                        SlotId = "mod_sight_front",
                    },
                    new Item
                    {
                        Id = "04ac5646bf20dba3d4928617",
                        Template = "5dfa3d7ac41b2312ea33362a",
                        ParentId = "065e966be13638b545da239b",
                        SlotId = "mod_sight_rear",
                    },
                },
            }
        },
        {
            "67801a528b7d993ae7b8ab9e",
            new Preset
            {
                Id = "67801a528b7d993ae7b8ab9e",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "MCM4 545",
                Parent = "07a5af8f7de0e93741ed09a8",
                Encyclopedia = "6628f50c78434b5effe5ced1",
                Items = new List<Item>
                {
                    new Item { Id = "07a5af8f7de0e93741ed09a8", Template = "6628f50c78434b5effe5ced1" },
                    new Item
                    {
                        Id = "f5a30471ccb1356eef6d1515",
                        Template = "59db3a1d86f77429e05b4e92",
                        ParentId = "07a5af8f7de0e93741ed09a8",
                        SlotId = "mod_pistol_grip",
                    },
                    new Item
                    {
                        Id = "ff5dc008c9be52eadcedd001",
                        Template = "6628f51c8df290deed8983e9",
                        ParentId = "07a5af8f7de0e93741ed09a8",
                        SlotId = "mod_magazine",
                    },
                    new Item
                    {
                        Id = "07a3a37f6cbe074fe03beae0",
                        Template = "6628f52f246a7106c3c3dd22",
                        ParentId = "07a5af8f7de0e93741ed09a8",
                        SlotId = "mod_reciever",
                    },
                    new Item
                    {
                        Id = "b174535f50afe8fc0ec4ba29",
                        Template = "5d120a10d7ad1a4e1026ba85",
                        ParentId = "07a5af8f7de0e93741ed09a8",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "287b0ac26bcf9d6ce8f745c0",
                        Template = "5d44334ba4b9362b346d1948",
                        ParentId = "07a5af8f7de0e93741ed09a8",
                        SlotId = "mod_charge",
                    },
                    new Item
                    {
                        Id = "0e5114fc4252115d252ab998",
                        Template = "584924ec24597768f12ae244",
                        ParentId = "07a3a37f6cbe074fe03beae0",
                        SlotId = "mod_scope",
                    },
                    new Item
                    {
                        Id = "eceff61728e0fd7d749fa9b6",
                        Template = "63d3ce0446bd475bcb50f55f",
                        ParentId = "07a3a37f6cbe074fe03beae0",
                        SlotId = "mod_barrel",
                    },
                    new Item
                    {
                        Id = "a014913d7d58829ab5eb74f4",
                        Template = "619b5db699fb192e7430664f",
                        ParentId = "07a3a37f6cbe074fe03beae0",
                        SlotId = "mod_handguard",
                    },
                    new Item
                    {
                        Id = "2d4b0e8d6e7580e06fccb5f2",
                        Template = "5c1780312e221602b66cc189",
                        ParentId = "07a3a37f6cbe074fe03beae0",
                        SlotId = "mod_sight_rear",
                    },
                    new Item
                    {
                        Id = "60375f7213800c3ed5373b1d",
                        Template = "5d120a28d7ad1a1c8962e295",
                        ParentId = "b174535f50afe8fc0ec4ba29",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "2dc80f7eb5f384d0ed143987",
                        Template = "5d02676dd7ad1a049e54f6dc",
                        ParentId = "eceff61728e0fd7d749fa9b6",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "d67869c5dcce0edd32ffe95a",
                        Template = "63d3ce281fe77d0f2801859e",
                        ParentId = "eceff61728e0fd7d749fa9b6",
                        SlotId = "mod_gas_block",
                    },
                    new Item
                    {
                        Id = "a7f0665b5404000439171f2d",
                        Template = "6269545d0e57f218e4548ca2",
                        ParentId = "a014913d7d58829ab5eb74f4",
                        SlotId = "mod_mount_000",
                    },
                    new Item
                    {
                        Id = "bdcff02ef006693d4e0ef2fb",
                        Template = "57cffb66245977632f391a99",
                        ParentId = "a014913d7d58829ab5eb74f4",
                        SlotId = "mod_foregrip",
                    },
                    new Item
                    {
                        Id = "85a6431d224222f10ea88f0a",
                        Template = "5c17804b2e2216152006c02f",
                        ParentId = "a014913d7d58829ab5eb74f4",
                        SlotId = "mod_sight_front",
                    },
                    new Item
                    {
                        Id = "d5c41299e72d04ffc83a293c",
                        Template = "5b07dd285acfc4001754240d",
                        ParentId = "a7f0665b5404000439171f2d",
                        SlotId = "mod_tactical",
                    },
                },
            }
        },
        {
            "67801a4ad3f4a15247709927",
            new Preset
            {
                Id = "67801a4ad3f4a15247709927",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "MCM4 57",
                Parent = "db1638d88584b10f9d26a85a",
                Encyclopedia = "6628f50c78434b5effe5ced1",
                Items = new List<Item>
                {
                    new Item { Id = "db1638d88584b10f9d26a85a", Template = "6628f50c78434b5effe5ced1" },
                    new Item
                    {
                        Id = "2e74e529aaa61f6ddd3b7a00",
                        Template = "59db3a1d86f77429e05b4e92",
                        ParentId = "db1638d88584b10f9d26a85a",
                        SlotId = "mod_pistol_grip",
                    },
                    new Item
                    {
                        Id = "11a5f28f7437e2a6a94ba5e4",
                        Template = "6628f5244c3764ecf474f23c",
                        ParentId = "db1638d88584b10f9d26a85a",
                        SlotId = "mod_magazine",
                    },
                    new Item
                    {
                        Id = "234a9d855f8c9a5cfeedb8be",
                        Template = "6628f534d238d16e71d27601",
                        ParentId = "db1638d88584b10f9d26a85a",
                        SlotId = "mod_reciever",
                    },
                    new Item
                    {
                        Id = "bbac5313efd899c0811acbba",
                        Template = "5d120a10d7ad1a4e1026ba85",
                        ParentId = "db1638d88584b10f9d26a85a",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "ec0758898bbde1c49d122168",
                        Template = "5d44334ba4b9362b346d1948",
                        ParentId = "db1638d88584b10f9d26a85a",
                        SlotId = "mod_charge",
                    },
                    new Item
                    {
                        Id = "b65a41799de1e0c5f468d67c",
                        Template = "584924ec24597768f12ae244",
                        ParentId = "234a9d855f8c9a5cfeedb8be",
                        SlotId = "mod_scope",
                    },
                    new Item
                    {
                        Id = "69c634ad2e379f37a47ce168",
                        Template = "63d3ce0446bd475bcb50f55f",
                        ParentId = "234a9d855f8c9a5cfeedb8be",
                        SlotId = "mod_barrel",
                    },
                    new Item
                    {
                        Id = "7f33ca92c798bdc57eebe365",
                        Template = "619b5db699fb192e7430664f",
                        ParentId = "234a9d855f8c9a5cfeedb8be",
                        SlotId = "mod_handguard",
                    },
                    new Item
                    {
                        Id = "1993a5ffc06f48adc03085aa",
                        Template = "5c1780312e221602b66cc189",
                        ParentId = "234a9d855f8c9a5cfeedb8be",
                        SlotId = "mod_sight_rear",
                    },
                    new Item
                    {
                        Id = "c033062ad56b6388f1a608bb",
                        Template = "5d120a28d7ad1a1c8962e295",
                        ParentId = "bbac5313efd899c0811acbba",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "530a099d127b2112a3b73237",
                        Template = "5d02676dd7ad1a049e54f6dc",
                        ParentId = "69c634ad2e379f37a47ce168",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "108bd14be99efc10e4b8b08e",
                        Template = "63d3ce281fe77d0f2801859e",
                        ParentId = "69c634ad2e379f37a47ce168",
                        SlotId = "mod_gas_block",
                    },
                    new Item
                    {
                        Id = "c21cd77a5fec6624a613e158",
                        Template = "6269545d0e57f218e4548ca2",
                        ParentId = "7f33ca92c798bdc57eebe365",
                        SlotId = "mod_mount_000",
                    },
                    new Item
                    {
                        Id = "30f87c907e73b109a6384866",
                        Template = "57cffb66245977632f391a99",
                        ParentId = "7f33ca92c798bdc57eebe365",
                        SlotId = "mod_foregrip",
                    },
                    new Item
                    {
                        Id = "194a2d1fc5e1bde3e79f0786",
                        Template = "5c17804b2e2216152006c02f",
                        ParentId = "7f33ca92c798bdc57eebe365",
                        SlotId = "mod_sight_front",
                    },
                    new Item
                    {
                        Id = "297ba80f5b85fa8b9945c61b",
                        Template = "5b07dd285acfc4001754240d",
                        ParentId = "c21cd77a5fec6624a613e158",
                        SlotId = "mod_tactical",
                    },
                },
            }
        },
        {
            "67801a42e3232969de230870",
            new Preset
            {
                Id = "67801a42e3232969de230870",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "MCM4 762",
                Parent = "590cb40366d027e18aed9c49",
                Encyclopedia = "6628f50c78434b5effe5ced1",
                Items = new List<Item>
                {
                    new Item { Id = "590cb40366d027e18aed9c49", Template = "6628f50c78434b5effe5ced1" },
                    new Item
                    {
                        Id = "659be0e4c26ec78e23ac47d2",
                        Template = "59db3a1d86f77429e05b4e92",
                        ParentId = "590cb40366d027e18aed9c49",
                        SlotId = "mod_pistol_grip",
                    },
                    new Item
                    {
                        Id = "3beec977e77f3c56daecec5c",
                        Template = "6628f51e54b4f59a7ade4ef9",
                        ParentId = "590cb40366d027e18aed9c49",
                        SlotId = "mod_magazine",
                    },
                    new Item
                    {
                        Id = "29633eb01a912c259c3a903a",
                        Template = "6628f531ae7642ade7384366",
                        ParentId = "590cb40366d027e18aed9c49",
                        SlotId = "mod_reciever",
                    },
                    new Item
                    {
                        Id = "0213bf270925e20bb9d68507",
                        Template = "5d120a10d7ad1a4e1026ba85",
                        ParentId = "590cb40366d027e18aed9c49",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "b081adf0e5d66f60977667c1",
                        Template = "5d44334ba4b9362b346d1948",
                        ParentId = "590cb40366d027e18aed9c49",
                        SlotId = "mod_charge",
                    },
                    new Item
                    {
                        Id = "6c621a5fe4a5694eba22d25e",
                        Template = "584924ec24597768f12ae244",
                        ParentId = "29633eb01a912c259c3a903a",
                        SlotId = "mod_scope",
                    },
                    new Item
                    {
                        Id = "9f509adb5a2e4d54217bd024",
                        Template = "63d3ce0446bd475bcb50f55f",
                        ParentId = "29633eb01a912c259c3a903a",
                        SlotId = "mod_barrel",
                    },
                    new Item
                    {
                        Id = "4d932906a9942488b8bb4ff9",
                        Template = "619b5db699fb192e7430664f",
                        ParentId = "29633eb01a912c259c3a903a",
                        SlotId = "mod_handguard",
                    },
                    new Item
                    {
                        Id = "bbfef1401a3648a353179a66",
                        Template = "5c1780312e221602b66cc189",
                        ParentId = "29633eb01a912c259c3a903a",
                        SlotId = "mod_sight_rear",
                    },
                    new Item
                    {
                        Id = "c0e23275364de197b5c2ec1d",
                        Template = "5d120a28d7ad1a1c8962e295",
                        ParentId = "0213bf270925e20bb9d68507",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "18aa717f44bfe3b93211352a",
                        Template = "5d02676dd7ad1a049e54f6dc",
                        ParentId = "9f509adb5a2e4d54217bd024",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "2619c8a27dd51846a85f1795",
                        Template = "63d3ce281fe77d0f2801859e",
                        ParentId = "9f509adb5a2e4d54217bd024",
                        SlotId = "mod_gas_block",
                    },
                    new Item
                    {
                        Id = "d7f7a81fd7138da7fc6eefc2",
                        Template = "6269545d0e57f218e4548ca2",
                        ParentId = "4d932906a9942488b8bb4ff9",
                        SlotId = "mod_mount_000",
                    },
                    new Item
                    {
                        Id = "fbad0e488ac155a022e9b15c",
                        Template = "57cffb66245977632f391a99",
                        ParentId = "4d932906a9942488b8bb4ff9",
                        SlotId = "mod_foregrip",
                    },
                    new Item
                    {
                        Id = "6c60d17a8643ef2d47db32a7",
                        Template = "5c17804b2e2216152006c02f",
                        ParentId = "4d932906a9942488b8bb4ff9",
                        SlotId = "mod_sight_front",
                    },
                    new Item
                    {
                        Id = "fe3abf02bb07f1076bc182b0",
                        Template = "5b07dd285acfc4001754240d",
                        ParentId = "d7f7a81fd7138da7fc6eefc2",
                        SlotId = "mod_tactical",
                    },
                },
            }
        },
        {
            "67801a3ccb05b9c7309b2ab3",
            new Preset
            {
                Id = "67801a3ccb05b9c7309b2ab3",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "MCM4 300",
                Parent = "1c91f6b9ea76c5e4a5e20820",
                Encyclopedia = "6628f50c78434b5effe5ced1",
                Items = new List<Item>
                {
                    new Item { Id = "1c91f6b9ea76c5e4a5e20820", Template = "6628f50c78434b5effe5ced1" },
                    new Item
                    {
                        Id = "3fa9e2b9b2a03de9841391ac",
                        Template = "59db3a1d86f77429e05b4e92",
                        ParentId = "1c91f6b9ea76c5e4a5e20820",
                        SlotId = "mod_pistol_grip",
                    },
                    new Item
                    {
                        Id = "c1b6fc96672fc3f981eba96c",
                        Template = "6628f511ea0ab10dd0c5e2d4",
                        ParentId = "1c91f6b9ea76c5e4a5e20820",
                        SlotId = "mod_magazine",
                    },
                    new Item
                    {
                        Id = "069f1dfbca3aa0937605f6da",
                        Template = "6628f529b9886fa300b47de7",
                        ParentId = "1c91f6b9ea76c5e4a5e20820",
                        SlotId = "mod_reciever",
                    },
                    new Item
                    {
                        Id = "984bbd95e5c32f3fb61d8bd5",
                        Template = "5d120a10d7ad1a4e1026ba85",
                        ParentId = "1c91f6b9ea76c5e4a5e20820",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "22a08b25360f51047d54cb7d",
                        Template = "5d44334ba4b9362b346d1948",
                        ParentId = "1c91f6b9ea76c5e4a5e20820",
                        SlotId = "mod_charge",
                    },
                    new Item
                    {
                        Id = "fcab1a04a63e102a34b63f09",
                        Template = "584924ec24597768f12ae244",
                        ParentId = "069f1dfbca3aa0937605f6da",
                        SlotId = "mod_scope",
                    },
                    new Item
                    {
                        Id = "fbb1e28b2c1a68bc0b06d721",
                        Template = "63d3ce0446bd475bcb50f55f",
                        ParentId = "069f1dfbca3aa0937605f6da",
                        SlotId = "mod_barrel",
                    },
                    new Item
                    {
                        Id = "ae6cb9ff02623a264f215201",
                        Template = "619b5db699fb192e7430664f",
                        ParentId = "069f1dfbca3aa0937605f6da",
                        SlotId = "mod_handguard",
                    },
                    new Item
                    {
                        Id = "de7b6df0da06d589706af299",
                        Template = "5c1780312e221602b66cc189",
                        ParentId = "069f1dfbca3aa0937605f6da",
                        SlotId = "mod_sight_rear",
                    },
                    new Item
                    {
                        Id = "1c9bd43c9ea6aaaa98c63025",
                        Template = "5d120a28d7ad1a1c8962e295",
                        ParentId = "984bbd95e5c32f3fb61d8bd5",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "093cca1b4622bed0ff7732a9",
                        Template = "5d02676dd7ad1a049e54f6dc",
                        ParentId = "fbb1e28b2c1a68bc0b06d721",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "3f2e53ae8dfee114ca2eca8c",
                        Template = "63d3ce281fe77d0f2801859e",
                        ParentId = "fbb1e28b2c1a68bc0b06d721",
                        SlotId = "mod_gas_block",
                    },
                    new Item
                    {
                        Id = "a673a6bc939e73b37ab7a220",
                        Template = "6269545d0e57f218e4548ca2",
                        ParentId = "ae6cb9ff02623a264f215201",
                        SlotId = "mod_mount_000",
                    },
                    new Item
                    {
                        Id = "f6a93b6925090c22babc901b",
                        Template = "57cffb66245977632f391a99",
                        ParentId = "ae6cb9ff02623a264f215201",
                        SlotId = "mod_foregrip",
                    },
                    new Item
                    {
                        Id = "6332c4ebba14b1506aa44e60",
                        Template = "5c17804b2e2216152006c02f",
                        ParentId = "ae6cb9ff02623a264f215201",
                        SlotId = "mod_sight_front",
                    },
                    new Item
                    {
                        Id = "dba320720ef8e1ea244b2ec6",
                        Template = "5b07dd285acfc4001754240d",
                        ParentId = "a673a6bc939e73b37ab7a220",
                        SlotId = "mod_tactical",
                    },
                },
            }
        },
        {
            "67801a3573c3693c4b84222c",
            new Preset
            {
                Id = "67801a3573c3693c4b84222c",
                Type = "Preset",
                ChangeWeaponName = false,
                Name = "MCM4 9x39",
                Parent = "6d6f86f937ce5dbdceb0849c",
                Encyclopedia = "6628f50c78434b5effe5ced1",
                Items = new List<Item>
                {
                    new Item { Id = "6d6f86f937ce5dbdceb0849c", Template = "6628f50c78434b5effe5ced1" },
                    new Item
                    {
                        Id = "38669ed0070dc3659e0545fb",
                        Template = "59db3a1d86f77429e05b4e92",
                        ParentId = "6d6f86f937ce5dbdceb0849c",
                        SlotId = "mod_pistol_grip",
                    },
                    new Item
                    {
                        Id = "33a4f57670c6b304bca280d6",
                        Template = "6628f5191c445ab1b8b8cdf5",
                        ParentId = "6d6f86f937ce5dbdceb0849c",
                        SlotId = "mod_magazine",
                    },
                    new Item
                    {
                        Id = "da0166c095789c0ae780c02e",
                        Template = "6628f52b821b61722b245c10",
                        ParentId = "6d6f86f937ce5dbdceb0849c",
                        SlotId = "mod_reciever",
                    },
                    new Item
                    {
                        Id = "429b986582cd49832504be8b",
                        Template = "5d120a10d7ad1a4e1026ba85",
                        ParentId = "6d6f86f937ce5dbdceb0849c",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "a05d2527c866550e40d21068",
                        Template = "5d44334ba4b9362b346d1948",
                        ParentId = "6d6f86f937ce5dbdceb0849c",
                        SlotId = "mod_charge",
                    },
                    new Item
                    {
                        Id = "3afc3b09e0610200db3a29cb",
                        Template = "584924ec24597768f12ae244",
                        ParentId = "da0166c095789c0ae780c02e",
                        SlotId = "mod_scope",
                    },
                    new Item
                    {
                        Id = "06f104bff96060698e392e3e",
                        Template = "63d3ce0446bd475bcb50f55f",
                        ParentId = "da0166c095789c0ae780c02e",
                        SlotId = "mod_barrel",
                    },
                    new Item
                    {
                        Id = "1cf0124af216f9de6d33450a",
                        Template = "619b5db699fb192e7430664f",
                        ParentId = "da0166c095789c0ae780c02e",
                        SlotId = "mod_handguard",
                    },
                    new Item
                    {
                        Id = "73a578d6a2adaf133423c81f",
                        Template = "5c1780312e221602b66cc189",
                        ParentId = "da0166c095789c0ae780c02e",
                        SlotId = "mod_sight_rear",
                    },
                    new Item
                    {
                        Id = "7e34178c167073d8b6184567",
                        Template = "5d120a28d7ad1a1c8962e295",
                        ParentId = "429b986582cd49832504be8b",
                        SlotId = "mod_stock",
                    },
                    new Item
                    {
                        Id = "02317d5330866a08475614c7",
                        Template = "5d02676dd7ad1a049e54f6dc",
                        ParentId = "06f104bff96060698e392e3e",
                        SlotId = "mod_muzzle",
                    },
                    new Item
                    {
                        Id = "d6935c70f7a32b06db7bbc4f",
                        Template = "63d3ce281fe77d0f2801859e",
                        ParentId = "06f104bff96060698e392e3e",
                        SlotId = "mod_gas_block",
                    },
                    new Item
                    {
                        Id = "76f35ca29c353d62aadc7c0b",
                        Template = "6269545d0e57f218e4548ca2",
                        ParentId = "1cf0124af216f9de6d33450a",
                        SlotId = "mod_mount_000",
                    },
                    new Item
                    {
                        Id = "234580a2be145e0f345e73d5",
                        Template = "57cffb66245977632f391a99",
                        ParentId = "1cf0124af216f9de6d33450a",
                        SlotId = "mod_foregrip",
                    },
                    new Item
                    {
                        Id = "7d1ebb169da76bd1d6a7d8af",
                        Template = "5c17804b2e2216152006c02f",
                        ParentId = "1cf0124af216f9de6d33450a",
                        SlotId = "mod_sight_front",
                    },
                    new Item
                    {
                        Id = "d5ebf47a092aeae293abfbb9",
                        Template = "5b07dd285acfc4001754240d",
                        ParentId = "76f35ca29c353d62aadc7c0b",
                        SlotId = "mod_tactical",
                    },
                },
            }
        },
    };
}
