# abandoned-net-project
Сто лет назад (в 2021 году) решил сделать онлайн игрушку, где ты покупаешь человечков, ставишь их на поле боя и они начинают делать что-то полезное. Проект умер из-за того, что у меня не было ни моделек, ни желания собирать поведение фигурок (ИИ) в той системе, которую я и сделал (стейт машина на скриптабельных объектах). 4 слоя серверов (Gate, Garden, Realm, Database) и клиент.
Если захотите забилдить и затестить: билдить нужно отдельно каждый слой (Gate, Garden, Realm, Database, Client),  Realm должен находиться внутри билда Garden (так как он его запускает по относительному пути), я не помню, какие значения ендпоинтов (ip_address:port) я оставил, так что все ip поменяйте на localhost (127.0.0.1).
Сторону сервера запускать по порядку: Датабаза, Гейт, Гарден. Со стороны клиента нужна регистрация (мыло проверяется регулярными выражениями, но не более того; пароли должны совпадать и быть длиной в 3 или более символа (вроде)). Можно запускать несколько клиентов на одном устройстве. В "катке" можно покупать и ставить только человечка (у него единственного есть адекватная иконка для витрины). 
