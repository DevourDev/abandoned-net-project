namespace DevourDev.Networking
{
    public enum ConnectionType
    {
        /// <summary>
        /// Задает вопросы и получает на них ответы.
        /// </summary>
        Requester,
        /// <summary>
        /// Слушает вопросы и отвечает на них.
        /// </summary>
        Responser,
        /// <summary>
        /// Получает (регулярную) информацию.
        /// </summary>
        MessagesListener,
        /// <summary>
        /// Сообщает (регулярную) информацию).
        /// </summary>
        Messager,
    }

}
