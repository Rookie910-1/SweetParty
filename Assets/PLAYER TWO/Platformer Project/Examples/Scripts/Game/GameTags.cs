
/// <summary>
/// 游戏物体标签常量类
/// </summary>
    public class GameTags
    {
        /// <summary>
        /// 玩家角色的标签
        /// 用于检测玩家对象（碰撞坚持、触发事件等）
        /// </summary>
        public static string Player = "Player";
        
        /// <summary>
        /// 敌人的标签
        /// 用于识别敌人对象
        /// </summary>
        public static string Enemy = "Enemy";
        
        /// <summary>
        /// 危险区域或者陷阱的标签
        /// 用于判定伤害区域
        /// </summary>
        public static string Hazard = "Hazard";
        
        /// <summary>
        /// 平台标签
        /// 用于识别可站立的平台对象
        /// </summary>
        public static string Platform = "Platform";
        
        /// <summary>
        /// 旗杆类的标签
        /// 用于特殊交互（滑杆、攀爬等）
        /// </summary>
        public static string Pole = "Pole";
        
        /// <summary>
        /// 面板的标签
        /// 可能是按钮、开关、UI交互面板
        /// </summary>
        public static string Panel = "Panel";       
        
        /// <summary>
        /// 弹簧的标签
        /// 用于跳跃辅助物
        /// </summary>
        public static string Spring = "Spring";
        
        /// <summary>
        /// 水体区域标签
        /// "Volume/Water"表示一个带有体积检测的水域
        /// </summary>
        public static string VolumeWater = "Volume/Water";
        
        /// <summary>
        /// 可交互轨道标签
        /// "Interactive/Rail" 可能用于滑行轨道或移动路径
        /// </summary>
        public static string InteractiveRail = "Interactive/Rail";
    }
