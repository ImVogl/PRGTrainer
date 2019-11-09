namespace PRGTrainer.Core.Model.Test
{
    /// <summary>
    /// Тип пользователя.
    /// </summary>
    public enum MemberType
    {
        /// <summary>
        /// Член комиссии с правом решающего голоса.
        /// </summary>
        Conclusive,

        /// <summary>
        /// Член комиссии с правом совещательного голоса.
        /// </summary>
        Consultative,

        /// <summary>
        /// Наблюдатель.
        /// </summary>
        Observer
    }
}