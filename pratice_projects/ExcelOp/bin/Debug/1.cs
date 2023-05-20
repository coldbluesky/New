public enum LoadingOutput
{
	[Description("上位机生命帧")]
	KeepAlive = 0,

	[Description("上位机停止信号")]
	PCStopSignal = 3,

	[Description("调度机器人安全信号")]
	DispatcherSafetySignal = 4,

	[Description("入料1#扫码完成信号")]
	IncomingMaterial1#ScanCodeCompleteSignal = 10,

	[Description("入料2#扫码完成信号")]
	IncomingMaterial2#ScanCodeCompleteSignal = 11,

	[Description("1#电芯虚拟码")]
	1#CellVirtualCode = 12,

	[Description("2#电芯虚拟码")]
	2#CellVirtualCode = 13,

	[Description("3#电芯虚拟码")]
	3#CellVirtualCode = 14,

	[Description("4#电芯虚拟码")]
	4#CellVirtualCode = 15,

	[Description("1#电芯扫码结果")]
	1#CellScanningResult = 16,

	[Description("2#电芯扫码结果")]
	2#CellScanningResult = 17,

	[Description("3#电芯扫码结果")]
	3#CellScanningResult = 18,

	[Description("4#电芯扫码结果")]
	4#CellScanningResult = 19,

	[Description("入料1#扫码枪连接异常")]
	IncomingMaterial1#ScanningGunConnectionException = 20,

	[Description("入料2#扫码枪连接异常")]
	IncomingMaterial2#ScanningGunConnectionException = 21,

	[Description("上位机假电池扫码完成信号")]
	UpperComputerFakeCellScaningCodeCompleteSignal = 25,

	[Description("假电池虚拟码")]
	FakeCellVirtualCode = 26,

	[Description("假电池扫码结果")]
	FakeCellScaningResult = 27,

	[Description("假电池扫码枪连接异常")]
	FakeCellScanningGunConnectionException = 28,

	[Description("1#平台放托盘完成信号")]
	1#PlacePalletOnPlatformCompleteSignal = 35,

	[Description("1#平台机器人放托盘中信号")]
	1#PlatformRobotPlacingPalletSignal = 36,

	[Description("1#托盘扫码完成信号")]
	1#PalletScanCodeCompleteSignal = 37,

	[Description("1#托盘虚拟码")]
	1#PalletVirtualCode = 38,

	[Description("1#托盘扫码结果")]
	1#PalletScanningResult = 39,

	[Description("1#托盘扫码枪连接异常")]
	1#PalletScanningGunConnectionException = 40,

	[Description("1#平台取托盘完成信号")]
	1#TakePalletFromPlatformCompleteSignal = 41,

	[Description("1#平台机器人取托盘中信号")]
	1#PlatformRobotTakingPalletSignal = 42,

	[Description("2#平台放托盘完成信号")]
	2#PlacePalletOnPlatformCompleteSignal = 45,

	[Description("2#平台机器人放托盘中信号")]
	2#PlatformRobotPlacingPalletSignal = 46,

	[Description("2#托盘扫码完成信号")]
	2#PalletScanCodeCompleteSignal = 47,

	[Description("2托盘虚拟码")]
	2#PalletVirtualCode = 48,

	[Description("2#托盘扫码结果")]
	2#PalletScanningResult = 49,

	[Description("2#托盘扫码枪连接异常")]
	2#PalletScanningGunConnectionException = 50,

	[Description("2#平台取托盘完成信号")]
	2#TakePalletFromPlatformCompleteSignal = 51,

	[Description("2#平台机器人取托盘中信号")]
	2#PlatformRobotTakingPalletSignal = 52,

	[Description("3#平台放托盘完成信号")]
	3#PlacePalletOnPlatformCompleteSignal = 55,

	[Description("3#平台机器人放托盘中信号")]
	3#PlatformRobotPlacingPalletSignal = 56,

	[Description("3#托盘扫码完成信号")]
	3#PalletScanCodeCompleteSignal = 57,

	[Description("3托盘虚拟码")]
	3#PalletVirtualCode = 58,

	[Description("3#托盘扫码结果")]
	3#PalletScanningResult = 59,

	[Description("3#托盘扫码枪连接异常")]
	3#PalletScanningGunConnectionException = 60,

	[Description("3#平台取托盘完成信号")]
	3#TakePalletFromPlatformCompleteSignal = 61,

	[Description("3#平台机器人取托盘中信号")]
	3#PlatformRobotTakingPalletSignal = 62,

}
