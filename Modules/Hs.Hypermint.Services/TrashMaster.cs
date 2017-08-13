using Hypermint.Base.Interfaces;
using System.IO;

namespace Hs.Hypermint.Services
{
    public class TrashMaster : ITrashMaster
    {
        public void RlFileToTrash(string fileName, string system, string mediaType, string romName)
        {
            var trashPath = GetRlTrashPath(system, mediaType, romName);

            MoveToTrash(trashPath, fileName);
        }

        public void HsFileToTrash(string fileName, string system, string mediaType, string romName)
        {
            var trashPath = GetHsTrashPath(system, mediaType);

            MoveToTrash(trashPath, fileName);
        }

        private void MoveToTrash(string trashPath, string fileName)
        {
            if (!Directory.Exists(trashPath))
                Directory.CreateDirectory(trashPath);

            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);

            string newFileName = trashPath + name + ext;

            int i = 1;
            while (File.Exists(newFileName))
            {
                newFileName = trashPath + name + i + ext;
                i++;
            }

            File.Move(fileName, newFileName);
        }

        private string GetRlTrashPath(string system, string mediaType, string romName) =>
            @"trash\" + system + "\\rl\\" + mediaType + "\\" + romName + "\\";

        public string GetHsTrashPath(string system, string mediaType) =>
            @"trash\" + system + "\\hs\\" + mediaType + "\\";

    }
}
