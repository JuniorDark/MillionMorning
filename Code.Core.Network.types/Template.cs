namespace Code.Core.Network.types;

public class Template
{
	public class Factory
	{
		public virtual Template Create(MessageReader reader)
		{
			return new Template(reader);
		}
	}

	private static readonly Factory[] ChildFactories;

	private readonly string _type;

	private readonly TemplateReference _reference;

	private const int TYPE_ID = 0;

	static Template()
	{
		ChildFactories = new Factory[146];
		ChildFactories[0] = new Factory();
		ChildFactories[1] = new ItemTemplate.Factory();
		ChildFactories[2] = new PlayerStateTemplate.Factory();
		ChildFactories[3] = new ProjectileTemplate.Factory();
		ChildFactories[4] = new MovableObjectTemplate.Factory();
		ChildFactories[5] = new TemplateCreatureAttack.Factory();
		ChildFactories[6] = new TemplateDamageSusceptibility.Factory();
		ChildFactories[7] = new ShopItem.Factory();
		ChildFactories[8] = new Spline.Factory();
		ChildFactories[9] = new GameplayTrigger.Factory();
		ChildFactories[10] = new GameplayObjectTemplate.Factory();
		ChildFactories[11] = new Teleport.Factory();
		ChildFactories[12] = new BossModeTemplate.Factory();
		ChildFactories[13] = new NpcTemplate.Factory();
		ChildFactories[14] = new EntityStateEffectTemplate.Factory();
		ChildFactories[15] = new RoomPresetTemplate.Factory();
		ChildFactories[16] = new CoinTemplate.Factory();
		ChildFactories[17] = new GemTemplate.Factory();
		ChildFactories[18] = new AmmoTemplate.Factory();
		ChildFactories[19] = new TeleportStoneTemplate.Factory();
		ChildFactories[20] = new MemberSubscriptionTemplate.Factory();
		ChildFactories[21] = new VoucherTemplateOld.Factory();
		ChildFactories[22] = new AbilityTemplate.Factory();
		ChildFactories[23] = new ConsumableTemplate.Factory();
		ChildFactories[24] = new OrbTemplate.Factory();
		ChildFactories[25] = new AchievementTemplate.Factory();
		ChildFactories[26] = new WearableTemplate.Factory();
		ChildFactories[27] = new VoucherTemplate.Factory();
		ChildFactories[28] = new HomeEquipmentTemplate.Factory();
		ChildFactories[29] = new LockBoxTemplate.Factory();
		ChildFactories[30] = new MysteryBoxTemplate.Factory();
		ChildFactories[31] = new VoucherPointTemplate.Factory();
		ChildFactories[32] = new ConverterTemplate.Factory();
		ChildFactories[33] = new ShopRoomTemplate.Factory();
		ChildFactories[34] = new ItemKitTemplate.Factory();
		ChildFactories[35] = new ArmorTemplate.Factory();
		ChildFactories[36] = new WieldableTemplate.Factory();
		ChildFactories[37] = new OffhandTemplate.Factory();
		ChildFactories[38] = new WeaponTemplate.Factory();
		ChildFactories[39] = new WieldableFoodTemplate.Factory();
		ChildFactories[40] = new MeleeWeaponTemplate.Factory();
		ChildFactories[41] = new RangedWeaponTemplate.Factory();
		ChildFactories[42] = new MeleeWeaponTemplate.Factory();
		ChildFactories[43] = new RangedWeaponTemplate.Factory();
		ChildFactories[44] = new HomeSurfaceTemplate.Factory();
		ChildFactories[45] = new FurnitureTemplate.Factory();
		ChildFactories[46] = new WallpaperTemplate.Factory();
		ChildFactories[47] = new FloorTemplate.Factory();
		ChildFactories[48] = new FloorFurnitureTemplate.Factory();
		ChildFactories[49] = new WallFurnitureTemplate.Factory();
		ChildFactories[50] = new AttachableFurnitureTemplate.Factory();
		ChildFactories[51] = new SeatTemplate.Factory();
		ChildFactories[52] = new HomeDeliveryBoxTemplate.Factory();
		ChildFactories[53] = new SeatTemplate.Factory();
		ChildFactories[54] = new HomeDeliveryBoxTemplate.Factory();
		ChildFactories[55] = new WeaponTemplate.Factory();
		ChildFactories[56] = new WieldableFoodTemplate.Factory();
		ChildFactories[57] = new MeleeWeaponTemplate.Factory();
		ChildFactories[58] = new RangedWeaponTemplate.Factory();
		ChildFactories[59] = new MeleeWeaponTemplate.Factory();
		ChildFactories[60] = new RangedWeaponTemplate.Factory();
		ChildFactories[61] = new WallpaperTemplate.Factory();
		ChildFactories[62] = new FloorTemplate.Factory();
		ChildFactories[63] = new FloorFurnitureTemplate.Factory();
		ChildFactories[64] = new WallFurnitureTemplate.Factory();
		ChildFactories[65] = new AttachableFurnitureTemplate.Factory();
		ChildFactories[66] = new SeatTemplate.Factory();
		ChildFactories[67] = new HomeDeliveryBoxTemplate.Factory();
		ChildFactories[68] = new SeatTemplate.Factory();
		ChildFactories[69] = new HomeDeliveryBoxTemplate.Factory();
		ChildFactories[70] = new MeleeWeaponTemplate.Factory();
		ChildFactories[71] = new RangedWeaponTemplate.Factory();
		ChildFactories[72] = new SeatTemplate.Factory();
		ChildFactories[73] = new HomeDeliveryBoxTemplate.Factory();
		ChildFactories[74] = new TemplateCreature.Factory();
		ChildFactories[75] = new BossTemplate.Factory();
		ChildFactories[76] = new MeleeCreatureAttackTemplate.Factory();
		ChildFactories[77] = new RangedCreatureAttackTemplate.Factory();
		ChildFactories[78] = new GameObjectSpline.Factory();
		ChildFactories[79] = new PlayerSpline.Factory();
		ChildFactories[80] = new InterTeleport.Factory();
		ChildFactories[81] = new IntraTeleport.Factory();
		ChildFactories[82] = new ArmorTemplate.Factory();
		ChildFactories[83] = new WieldableTemplate.Factory();
		ChildFactories[84] = new OffhandTemplate.Factory();
		ChildFactories[85] = new WeaponTemplate.Factory();
		ChildFactories[86] = new WieldableFoodTemplate.Factory();
		ChildFactories[87] = new MeleeWeaponTemplate.Factory();
		ChildFactories[88] = new RangedWeaponTemplate.Factory();
		ChildFactories[89] = new MeleeWeaponTemplate.Factory();
		ChildFactories[90] = new RangedWeaponTemplate.Factory();
		ChildFactories[91] = new HomeSurfaceTemplate.Factory();
		ChildFactories[92] = new FurnitureTemplate.Factory();
		ChildFactories[93] = new WallpaperTemplate.Factory();
		ChildFactories[94] = new FloorTemplate.Factory();
		ChildFactories[95] = new FloorFurnitureTemplate.Factory();
		ChildFactories[96] = new WallFurnitureTemplate.Factory();
		ChildFactories[97] = new AttachableFurnitureTemplate.Factory();
		ChildFactories[98] = new SeatTemplate.Factory();
		ChildFactories[99] = new HomeDeliveryBoxTemplate.Factory();
		ChildFactories[100] = new SeatTemplate.Factory();
		ChildFactories[101] = new HomeDeliveryBoxTemplate.Factory();
		ChildFactories[102] = new WeaponTemplate.Factory();
		ChildFactories[103] = new WieldableFoodTemplate.Factory();
		ChildFactories[104] = new MeleeWeaponTemplate.Factory();
		ChildFactories[105] = new RangedWeaponTemplate.Factory();
		ChildFactories[106] = new MeleeWeaponTemplate.Factory();
		ChildFactories[107] = new RangedWeaponTemplate.Factory();
		ChildFactories[108] = new WallpaperTemplate.Factory();
		ChildFactories[109] = new FloorTemplate.Factory();
		ChildFactories[110] = new FloorFurnitureTemplate.Factory();
		ChildFactories[111] = new WallFurnitureTemplate.Factory();
		ChildFactories[112] = new AttachableFurnitureTemplate.Factory();
		ChildFactories[113] = new SeatTemplate.Factory();
		ChildFactories[114] = new HomeDeliveryBoxTemplate.Factory();
		ChildFactories[115] = new SeatTemplate.Factory();
		ChildFactories[116] = new HomeDeliveryBoxTemplate.Factory();
		ChildFactories[117] = new MeleeWeaponTemplate.Factory();
		ChildFactories[118] = new RangedWeaponTemplate.Factory();
		ChildFactories[119] = new SeatTemplate.Factory();
		ChildFactories[120] = new HomeDeliveryBoxTemplate.Factory();
		ChildFactories[121] = new PlayerSpline.Factory();
		ChildFactories[122] = new WeaponTemplate.Factory();
		ChildFactories[123] = new WieldableFoodTemplate.Factory();
		ChildFactories[124] = new MeleeWeaponTemplate.Factory();
		ChildFactories[125] = new RangedWeaponTemplate.Factory();
		ChildFactories[126] = new MeleeWeaponTemplate.Factory();
		ChildFactories[127] = new RangedWeaponTemplate.Factory();
		ChildFactories[128] = new WallpaperTemplate.Factory();
		ChildFactories[129] = new FloorTemplate.Factory();
		ChildFactories[130] = new FloorFurnitureTemplate.Factory();
		ChildFactories[131] = new WallFurnitureTemplate.Factory();
		ChildFactories[132] = new AttachableFurnitureTemplate.Factory();
		ChildFactories[133] = new SeatTemplate.Factory();
		ChildFactories[134] = new HomeDeliveryBoxTemplate.Factory();
		ChildFactories[135] = new SeatTemplate.Factory();
		ChildFactories[136] = new HomeDeliveryBoxTemplate.Factory();
		ChildFactories[137] = new MeleeWeaponTemplate.Factory();
		ChildFactories[138] = new RangedWeaponTemplate.Factory();
		ChildFactories[139] = new SeatTemplate.Factory();
		ChildFactories[140] = new HomeDeliveryBoxTemplate.Factory();
		ChildFactories[141] = new MeleeWeaponTemplate.Factory();
		ChildFactories[142] = new RangedWeaponTemplate.Factory();
		ChildFactories[143] = new SeatTemplate.Factory();
		ChildFactories[144] = new HomeDeliveryBoxTemplate.Factory();
		ChildFactories[145] = new SkillItemTemplate.Factory();
	}

	public static Template Create(int id, MessageReader reader)
	{
		return ChildFactories[id].Create(reader);
	}

	public virtual int GetTypeId()
	{
		return 0;
	}

	protected Template(MessageReader reader)
	{
		_type = reader.ReadString();
		_reference = new TemplateReference(reader);
	}

	protected Template(string type, TemplateReference reference)
	{
		_type = type;
		_reference = reference;
	}

	public string GetTemplateType()
	{
		return _type;
	}

	public TemplateReference GetReference()
	{
		return _reference;
	}

	public virtual int Size()
	{
		return 2 + MessageWriter.GetSize(_type) + _reference.Size();
	}

	public virtual void Write(MessageWriter writer)
	{
		writer.WriteString(_type);
		_reference.Write(writer);
	}
}
