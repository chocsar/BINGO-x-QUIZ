/// <summary>
/// ユーザーのビンゴの状態を定義した定数クラス
/// </summary>
public class UserBingoStatus
{
    public const string Default = "default";
    public const string Reach = "reach";
    public const string Bingo = "bingo";

    /// <summary>
    /// CanOpen状態のセルを含めてビンゴになる状態（FirebaseにはBingoとして保存）
    /// </summary>
    public const string PreBingo = "prebingo";

    /// <summary>
    /// CanOpen状態のセルを含めてリーチになる状態（FirebaseにはReachとして保存）
    /// </summary>
    public const string PreReach = "prereach";

}
