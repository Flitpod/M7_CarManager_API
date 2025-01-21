namespace M7_CarManager.Models
{
    public class FbModel
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public Picture picture { get; set; }
        public string email { get; set; }
        public string id { get; set; }
        public class Picture
        {
            public PictureData data { get; set; }
            public class PictureData
            {
                public int height { get; set; }
                public bool is_silhouette { get; set; }
                public string url { get; set; }
                public int width { get; set; }
            }
        }
    }
}
