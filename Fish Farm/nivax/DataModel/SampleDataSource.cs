using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace FoodVariable.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : FoodVariable.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get { return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }



        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "In Pakistan and India every woman knows that bride is incomplete without applying beautiful and stunning mehndi designs on her hands, feet and arms. These days, applying mehndi becomes popular fashion. Every year, numerous mehndi designs are coming for women and young girls. In this post, we are presenting latest and exclusive mehndi designs 2013 for women. Women and girls can apply these mehndi designs on their hands, arms and feet. All mehndi designs 2013 are simply stunning and magnificent. These mehndi designs 2013 include different types of designs like floral designs, peacock designs and many more. If we talk about these mehndi designs then some mehndi designs are extremely beautiful but difficult. So women can apply them with the help of professional mehndi artist. On the other hand, some of them are simple then even girls can easily apply them without taking any help.");

            var group1 = new SampleDataGroup("Group-1",
                 "FreshWater Fish",
                 "Group Subtitle: 1",
                 "Assets/DarkGray.png",
                 "Group Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs. In this style we can use different styles of mehndi like Black mehndi is used as outline, fillings with the normal henna mehndi. We can also include sparkles as a final coating to make the henna design more attractive.");

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item1",
                 "Puffer Fish",
                 "Puffer Fish",
                 "Assets/HubPage/HubpageImage2.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "The puffer fish is one of the most interesting and unusual species of fish with several unique characteristics to its credit. When it feels threatened, it puffs up to double its size by swallowing water or air. There is little questioning the fact that the puffer fish is widely known for its ability to inflate itself, but one has to understand that this is an adaptation which compensates their inability to swim fast. Not to forget, some puffer fish species have tiny spines which only become visible when they inflate themselves.",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item2",
                 "Piranha",
                 "Piranha",
                 "Assets/HubPage/HubpageImage3.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "Piranhas are normally about 14 to 26 cm long (5.5 to 10.25 inches), although some specimens have been reported to be up to 43 cm (17.0 inches) in length.Serrasalmus, Pristobrycon, Pygocentrus and Pygopristis are most easily recognized by their unique dentition. All piranhas have a single row of sharp teeth in both jaws; the teeth are tightly packed and interlocking (via small cusps) and are used for rapid puncture and shearing. Individual teeth are typically broadly triangular, pointed and blade-like (flat in profile). There is minor variation in the number of cusps; in most species, the teeth are tricuspid with a larger middle cusp which makes the individual teeth appear markedly triangular. The exception is Pygopristis, which has pentacuspid teeth and a middle cusp usually only slightly larger than the other cusps. In the scale-eating Catoprion, the shape of their teeth is markedly different and the premaxillary teeth are in two rows, as in most other serrasalmines.",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item3",
                 "Green Swordtail",
                 "Green Swordtail",
                 "Assets/HubPage/HubpageImage4.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "The green swordtail (Xiphophorus hellerii) is a species of freshwater fish in family Poeciliidae of order Cyprinodontiformes. A live-bearer, it is closely related to the southern platyfish or platy (X. maculatus) and can crossbreed with it. It is native to an area of North and Central America stretching from Veracruz, Mexico, to northwestern Honduras.The male green swordtail grows to a maximum overall length of 14 cm (5.5 in) and the female to 16 cm (6.3 in). The name swordtail is derived from the elongated lower lobe of the male's caudal fin (tailfin). Sexual dimorphism is moderate, with the female being larger than the male, but lacking the sword. The wild form is olive green in color, with a red or brown lateral stripe and speckles on the dorsal and, sometimes, caudal fins. The male's sword is yellow, edged in black below. Captive breeding has produced many color varieties, including black, red, and many patterns thereof, for the aquarium hobby.\n\nThe green swordtail prefers swift-flowing, heavily-vegetated rivers and streams, but is also found in warm springs and canals. Omnivorous, its diet includes both plants and small crustaceans, insects, and annelid worms.",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item4",
                 "Archerfish",
                 "Archerfish",
                 "Assets/HubPage/HubpageImage5.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "The archerfish (spinner fish or archer fish) are a family (Toxotidae) of fish known for their habit of preying on land based insects and other small animals by literally shooting them down with water droplets from their specialized mouths. The family is small, consisting of seven species in the genus Toxotes; which typically inhabit brackish waters of estuaries and mangroves, but can also be found in the open ocean as well as far upstream in fresh water[1] They can be found from India to the Philippines, Australia, and Polynesia.\n\nArcherfish or Spinnerfish bodies are deep and laterally compressed, with the dorsal fin, and the profile a straight line from dorsal fin to mouth. The mouth is protractile, and the lower jaw juts out. Sizes are generally small,about 5–10 cm but T. chatareus can reach 40 centimetres (16 in).",
                 69,
                 70,
                 group1));

            group1.Items.Add(new SampleDataItem("Landscape-Group-1-Item5",
                 "Cichlid",
                 "Cichlid",
                 "Assets/HubPage/HubpageImage6.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "Cichlids are fish from the family Cichlidae in the order Perciformes. Cichlids are members of a group known as the Labroidei, along with the wrasses (Labridae), damselfishes (Pomacentridae), and surfperches (Embiotocidae).[1] This family is both large and diverse. At least 1,650 species have been scientifically described,[2] making it one of the largest vertebrate families. New species are discovered annually, and many species remain undescribed. The actual number of species is therefore unknown, with estimates varying between 2,000 and 3,000.[3] Cichlids are among the most popular freshwater fish kept in the home aquarium.",
                 69,
                 35,
                 group1));

            

            this.AllGroups.Add(group1);

            var group2 = new SampleDataGroup("Group-2",
                "Marine Fish",
                "Group Subtitle: 2",
                "Assets/DarkGray.png",
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group2.Items.Add(new SampleDataItem("Big-Group-2-Item1",
                "Banggai cardinalfish",
                "Banggai cardinalfish",
                "Assets/HubPage/HubpageImage7.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "This species grows up to 8 centimetres (3 in) total length. It has a distinctive contrasting pattern of black and light bars with white spots. This species is easily differentiated from all other cardinalfish by its tasseled first dorsal fin, elongate anal and second dorsal fin rays, deeply forked caudal fin, and color pattern consisting of three black bars across the head and body and prominent black anterior edges on the anal and second dorsal fin.[3] Males can be differentiated from females by a conspicuous enlarged oral cavity, which is apparent only when they are brooding.",
                69,
                70,
                group2));

            group2.Items.Add(new SampleDataItem("Landscape-Group-2-Item2",
                "Acanthuridae",
                "Acanthuridae",
                "Assets/HubPage/HubpageImage8.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Acanthuridae (thorn tails) is the family of surgeonfishes, tangs, and unicornfishes. The family is composed of marine fish living in tropical seas, usually around coral reefs. Many of the species are brightly colored and popular for aquaria.The distinctive characteristic of the family is the scalpellike spines, one or more on either side of the tail, which are dangerously sharp. The dorsal, anal and caudal fins are large, extending for most of the length of the body. The small mouths have a single row of teeth used for grazing on algae.\n\nSurgeonfishes sometimes feed as solitary individuals, but they also often travel and feed in schools. It has been suggested that feeding in schools is a mechanism for overwhelming the highly aggressive defense responses of small territorial damselfishes that vigorously guard small patches of algae on coral reefs.\n\nMost species are relatively small and have a maximum length of 15–40 cm (6–16 in), but some members of the genus Acanthurus, some members of the genus Prionurus, and most members of the genus Naso can grow larger, with the whitemargin unicornfish (N. annulatus), the largest species in the family, reaching a length of up to 1 metre (3 ft 3 in). These fishes can grow quickly in aquariums so it is advisable to check the average growth size and suitability before adding to a marine aquarium.",
                69,
                35,
                group2));

            group2.Items.Add(new SampleDataItem("Medium-Group-2-Item3",
                "Yellow Tang",
                "Yellow Tang",
                "Assets/HubPage/HubpageImage9.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "The yellow tang (Zebrasoma flavescens) reaches a diameter of nearly 8 inches in the sea; aquarium specimens seldom exceed 6 inches. Nevertheless, provide a tank 4 feet or more in length for these active swimmers.\n\nIn the reef aquarium, the yellow tang earns its keep by grazing on filamentous algae, helping to keep the rocks and decorations free of excessive growth.\n\nAlthough large, well-established specimens may be aggressive, particularly under crowded conditions or in a tank that is too small to make them feel comfortable, this species seldom presents a behavior problem. Therefore, yellow tangs can be housed with most other fish, including smaller species. Because these fish are not predatory toward other fish, they are suitable for community displays. In the tank tangs feed almost exclusively on algae and small invertebrates.\n\nNormally found in loose aggregations of two to many individuals, if you introduce all specimens into the aquarium at the same time, they get along fine. As the focus of a species tank, nothing rivals a group of yellow tangs. They display both a stunning color and active behaviors. The species aquarium, however, must be large. I recommend nothing less than a 6-foot-long, 125-gallon tank or larger.\n\nYellow tangs are found in much of the Pacific, occurring from Japan to Hawaii north of the equator. Typically, they live on outer reefs with dense coral stands, most commonly of the branching genera Acropora and Pocillopora. These fish may also be found in lagoons. Yellow tangs range in water depths of about 10 feet to more than 100 feet.",
                41,
                41,
                group2));

            group2.Items.Add(new SampleDataItem("Medium-Group-2-Item4",
                "Hawkfish",
                "Hawkfish",
                "Assets/HubPage/HubpageImage09.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "The hawkfishes are strictly tropical, perciform marine fishes of the family Cirrhitidae. Associated with the coral reefs of the western and eastern Atlantic and Indo-Pacific, the hawkfish family contains 12 genera and 32 species. They share many morphological features with the scorpionfish of the family Scorpaenidae.\n\nHawkfishes have large heads with thick, somewhat elongated bodies. Their dorsal fins are merged, with the first consisting of ten connected spines. At the tip of each spine are several trailing filaments, hence the family name Cirrhitidae, from the Latin cirrus meaning fringe. Their tail fins are rounded and truncated, and their pectoral fins are enlarged and scaleless. Their scales may be cycloid or ctenoid. Most hawkfishes are small, from about 7-15 cm in length. The largest species, the giant hawkfish (Cirrhitus rivulatus) attains a length of 60 cm and a weight of 4 kg. A commercial fishery exists for the larger species, as they are considered excellent food fishes.\n\nThe vibrant colours exhibited by most hawkfishes have won them popularity in the aquaria hobby, aided by the fishes' reputation for unproblematic upkeep and easy acclimation to tank life. Popularly kept species include the longnose hawkfish (Oxycirrhites typus), coloured in a red and pink crosshatch pattern, and the flame hawkfish (Neocirrhites armatus).",
                41,
                41,
                group2));

            
            this.AllGroups.Add(group2);


            

            var group3 = new SampleDataGroup("Group-3",
               "Migrating Fish",
               "Group Subtitle: 2",
               "Assets/DarkGray.png",
               "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group3.Items.Add(new SampleDataItem("Big-Group-3-Item1",
                "Tuna",
                "Tuna",
                "Assets/HubPage/HubpageImage10.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "A tuna is a saltwater finfish that belongs to the tribe Thunnini, a sub-grouping of the mackerel family (Scombridae) – which together with the tunas, also includes the bonitos, mackerels, and Spanish mackerels. Thunnini comprises fifteen species across five genera,[1] the sizes of which vary greatly, ranging from the bullet tuna (max. length: 50 cm (1.6 ft), weight: 1.8 kg (4 lb)) up to the Atlantic bluefin tuna (max. length: 4.6 m (15 ft), weight: 684 kg (1,508 lb)). The bluefin averages 2 m (6.6 ft), and is believed to live for up to 50 years.\n\nTheir circulatory and respiratory systems are unique among fish, enabling them to maintain a body temperature higher than the surrounding water. An active and agile predator, the tuna has a sleek, streamlined body, and is among the fastest-swimming pelagic fish – the yellowfin tuna, for example, is capable of speeds of up to 75 km/h (47 mph).[2] Found in warm seas, it is extensively fished commercially and is popular as a game fish. As a result of over-fishing, stocks of some tuna species, such as the Southern bluefin tuna, have been reduced dangerously close to the point of extinction.",
                69,
                70,
                group3));

            group3.Items.Add(new SampleDataItem("Landscape-Group-3-Item2",
                "Dolphin",
                "Dolphin",
                "Assets/HubPage/HubpageImage11.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Dolphins are marine mammals closely related to whales and porpoises. There are almost forty species of dolphin in 17 genera. They vary in size from 1.2 m (4 ft) and 40 kg (90 lb) (Maui's dolphin), up to 9.5 m (30 ft) and 10 tonnes (9.8 long tons; 11 short tons) (the orca or killer whale). They are found worldwide, mostly in the shallower seas of the continental shelves, and are carnivores, eating mostly fish and squid. The family Delphinidae is the largest in the Cetacean order, and evolved relatively recently, about ten million years ago, during the Miocene. Dolphins are among the most intelligent animals, and their often friendly appearance, an artifact of the smile of their mouthline, and seemingly playful attitude have made them very popular in human culture.",
                69,
                35,
                group3));

            group3.Items.Add(new SampleDataItem("Medium-Group-3-Item3",
                "Bullshark",
                "Bullshark",
                "Assets/HubPage/HubpageImage12.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "The bull shark, Carcharhinus leucas, also known as the Zambezi shark (UK: Zambesi shark) or unofficially Zambi in Africa and Nicaragua shark in Nicaragua, is a shark commonly found worldwide in warm, shallow waters along coasts and in rivers. The bull shark is known for its aggressive nature, predilection for warm shallow water, and presence in brackish and freshwater systems including estuaries and rivers.\n\nThe bull shark can thrive in both saltwater and freshwater and can travel far up rivers. They have even been known to travel as far up as Kentucky in the Ohio River, although there have been few recorded attacks. They are probably responsible for the majority of near-shore shark attacks, including many attacks attributed to other species.Bull sharks are not actually true freshwater sharks, despite their ability to survive in freshwater habitats (unlike the river sharks of the genus Glyphis).",
                41,
                41,
                group3));
            group3.Items.Add(new SampleDataItem("Medium-Group-3-Item4",
               "Trout",
               "Trout",
               "Assets/HubPage/HubpageImage13.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "Trout is the name for a number of species of freshwater fish belonging to the genera Oncorhynchus, Salmo and Salvelinus, all of the subfamily Salmoninae of the family Salmonidae. The word trout is also used as part of the name of some non-salmonid fish such as Cynoscion nebulosus, the spotted seatrout or speckled trout.\n\nTrout are closely related to salmon and char (or charr): species termed salmon and char occur in the same genera as do trout (Oncorhynchus - Pacific salmon and trout, Salmo - Atlantic salmon and various trout, Salvelinus - char and trout).\n\nMost trout such as Lake trout live in freshwater lakes and/or rivers exclusively, while there are others such as the Rainbow trout which may either live out their lives in fresh water, or spend two or three years at sea before returning to fresh water to spawn, a habit more typical of salmon. A rainbow trout that spends time in the ocean is called a steelhead.\n\nTrout are an important food source for humans and wildlife including brown bears, birds of prey such as eagles, and other animals. They are classified as oily fish.",
               41,
               41,
               group3));

            this.AllGroups.Add(group3);


         



            var group4 = new SampleDataGroup("Group-4",
               "Aquarium Fish",
               "Group Subtitle: 2",
               "Assets/DarkGray.png",
               "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item1",
               "Clown loach",
               "Clown loach",
               "Assets/HubPage/HubpageImage14.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "Information about the maximum size of the clown loach varies, with some estimates ranging from 12 to 16 inches (30 to 40 cm), and with typical adult sizes ranging from 7 to 10 inches (18 to 26 cm).[5] The fish's body is long and laterally compressed, with an arched dorsal surface and a flat ventral surface. Its head is relatively large and its mouth faces downward with thick, fleshy lips, and four pairs of barbels. The barbels on the lower jaw are small and difficult to see. Clown loaches can make clicking sounds when they are happy or mating.\n\nThe body is whitish-orange to reddish-orange, with three thick, black, triangular, vertical bands. The anterior band runs from the top of the head and through the eye, the medial band lies between the head and the dorsal fin, and wraps around to the ventral surface, and the posterior band covers almost all of the caudal peduncle and extends to the anal fin. There is some regional color variation within the species; the pelvic fins on fish from Borneo are reddish orange and black, while the pelvic fins on fish from Sumatra are entirely reddish orange.\n\nThe fish is sexually dimorphic, with females being slightly plumper than males. In addition, the tips of the tail on the male curve inwards slightly, whereas the females have straight tips.\n\nThe fish has a movable spine that lies in a groove below the eye, which may be extended as a defense mechanism. The spine may cause a painful wound, but is not venomous. It also may be used as a predation tool as it is set close to the mouth.",
               41,
               41,
               group4));

            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item2",
                "Balashark",
                "Balashark",
                "Assets/HubPage/HubpageImage15.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Bala sharks are popular aquarium fish.[4] These fish are generally peaceful and good companions to many other types of tropical fish.[4] The nickname shark is used because of their torpedo shaped bodies and long fins. They are not actual sharks. Bala sharks are widely available in most pet stores, but these fish may grow to a size too large for the home aquarium. Also the Bala shark is a jumper fish and may injure itself on the lid of a tank.\n\nThey are a hardy fish that will tolerate temperature changes, pH changes, and other factors to which other fish may be sensitive. The water pH should be 6.0–8.0. The preferable water hardness for this species is soft to medium (5.0–12.0 dGH). Water temperature should be kept between 22–28°C (72–82°F).The Bala shark prefers to be kept in groups of two or more specimens (although they can survive alone).These fish require a covered aquarium as they are very skilled jumpers.\n\nVery young Bala sharks are sometimes kept in small aquaria. However, given their adult size, schooling behavior, and swimming speed, the fish quickly grow to need much more room. Hobbyists continue to debate over acceptable minimum tank sizes, but generally recommend at least a 6 foot tank. FishBase lists a minimum of 150 cm (5 ft).Many believe the fish is simply too large and too active to be kept in residential aquaria at all; only enormous, custom-built tanks are acceptable, if any tank at all is. Indoor ponds are also considered feasible housing options and may be better suited to the average aquarist. This fish's habitat is often destroyed, making these fish rare in the wild.",
                41,
                41,
                group4));
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item3",
               "Koi Carp",
               "Koi Carp",
               "Assets/HubPage/HubpageImage16.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "Carp are a large group of fish originally found in Central Europe and Asia. Various carp species were originally domesticated in East Asia, where they were used as food fish. The ability of carp to survive and adapt to many climates and water conditions allowed the domesticated species to be propagated to many new locations, including Japan. Natural color mutations of these carp would have occurred across all populations. Carp were first bred for color mutations in China more than a thousand years ago, where selective breeding of the Prussian carp (Carassius gibelio) led to the development of the goldfish.\n\nThe common carp was aquacultured as a food fish at least as far back as the fifth century BC in China, and in the Roman Empire during the spread of Christianity in Europe.[5] Common carp were bred for color in Japan in the 1820s, initially in the town of Ojiya in the Niigata prefecture on the northeastern coast of Honshu island. By the 20th century, a number of color patterns had been established, most notably the red-and-white Kohaku. The outside world was not aware of the development of color variations in koi until 1914, when the Niigata koi were exhibited in the annual exposition in Tokyo. At that point, interest in koi exploded throughout Japan. The hobby of keeping koi eventually spread worldwide. They are now commonly sold in most pet stores, with higher-quality fish available from specialist dealers.\n\nExtensive hybridization between different populations has muddled the historical zoogeography of the common carp. However, scientific consensus is that there are at least two subspecies of the common carp, one from Western Eurasia (Cyprinus carpio carpio) and another from East Asia (Cyprinus carpio haematopterus).[8] One recent study on the mitochondrial DNA of various common carp indicate that koi are of the East Asian subspecies.[8] However, another recent study on the mitochondrial DNA of koi have found that koi are descended from multiple lineages of common carp from both Western Eurasian and East Asian varieties.[9] This could be the result of koi being bred from a mix of East Asian and Western Eurasian carp varieties, or being bred exclusively from East Asian varieties and being subsequently hybridized with Western Eurasian varieties (the butterfly koi is one known product of such a cross). Which is true has not been resolved.\n\nIt was from this handful of Koi breeds that all other Nishikigoi types were bred, with the exception of the Ogon variety (single colored, metallic Koi) which wasn't developed until recently. The last development of this early time was a great breakthrough in Koi breeding and is still revered as one of the most traditional of Koi breeds. A tri-colored Koi called a Taisho Sanshoku, more commonly known as the Sanke, was first seen during the Meiji era (1868-1912). Though it is not known who first developed this breed, the Sanke was exhibited for the first time in 1915, when the Koi was about 15 years old.",
               41,
               41,
               group4));
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item4",
               "Marine hatchetfish",
               "Marine hatchetfish",
               "Assets/HubPage/HubpageImage17.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "Found in tropical and subtropical waters of the Atlantic, Pacific and Indian Oceans, marine hatchetfishes range in size from Polyipnus danae at 2.8 cm (1.1 in) to the c.12 cm (4.7 in)-long Giant hatchetfish (Argyropelecus gigas). They are small deep-sea fishes which have evolved a peculiar body shape and like their relatives have bioluminescent photophores. The latter allow them to use counterillumination to escape predators that lurk in the depths: by matching the light intensity with the light penetrating the water from above, the fish does not appear darker if seen from below. They typically occur at a few hundred meters below the surface, but their entire depth range spans from 50 to 1,500 meters deep.\n\nThe body is deep and laterally extremely compressed, somewhat resembling a hatchet (with the thorax being the blade and the caudal peduncle being the handle). The genus Polyipnus is rounded, the other two – in particular Sternoptyx – decidedly angular if seen from the side. Their pelvis is rotated to a vertical position. The mouth is located at the tip of the snout and directed almost straight downwards.\n\nTheir scales are silvery, delicate and easily abraded. In some species, such as the Highlight Hatchetfish (Sternoptyx pseudobscura), large sections of the body at the base of the anal fin and/or caudal fin are transparent. They have perpendicular spines and blade-like pterygiophores in front of the dorsal fin. The anal fin has 11-19 rays and in some species is divided in two parts; almost all have an adipose fin. Their large, sometimes tube-shaped eyes can collect the faintest of light and focus well on objects both close and far. They are directed somewhat upwards, most conspicuously in the genus Argyropelecus. This allows to discern the silhouettes of prey moving overhead against the slightly brighter upper waters.",
               41,
               41,
               group4));
            this.AllGroups.Add(group4);



        }
    }
}
