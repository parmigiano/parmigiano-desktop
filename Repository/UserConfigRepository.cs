using Parmigiano.Core;
using Parmigiano.Interface;
using System;
using System.IO;

namespace Parmigiano.Repository
{
    public class UserConfigRepository : IUserConfigRepository
    {
        public void Delete()
        {
            try
            {
                if (File.Exists(Config.Current.CONFIG_USER_PATH))
                {
                    File.Delete(Config.Current.CONFIG_USER_PATH);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления user.conf: {ex.Message}");
            }
        }

        public string Load()
        {
            try
            {
                if (File.Exists(Config.Current.CONFIG_USER_PATH))
                {
                    return File.ReadAllText(Config.Current.CONFIG_USER_PATH).Trim();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения user.conf: {ex.Message}");
            }

            return null;
        }

        public void Save(string data)
        {
            try
            {
                if (!Directory.Exists(Config.Current.APP_FOLDER_PATH))
                {
                    Directory.CreateDirectory(Config.Current.APP_FOLDER_PATH);
                }

                File.WriteAllText(Config.Current.CONFIG_USER_PATH, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка записи user.conf: {ex.Message}");
            }
        }
    }
}
