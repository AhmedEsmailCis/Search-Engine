using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Search__Engine
{
    public partial class Form1 : Form
    {
        static bool flag = true;
        static Stemmer stemmer = new Stemmer();
        static List<string> query_without_stopwords = new List<string> { };
        static SqlConnection con;
        static SqlCommand cmd;
        struct word_and_docID
        {
            public int word_number;
            public Decimal document_ID;
            public Decimal frequency;
            public string positions;
        }
        static word_and_docID term;
        public struct retreival_documents
        {
            public int score;
            public Decimal docID;
            public Decimal freq;
        }
        List<word_and_docID> all_documents = new List<word_and_docID> { };
        List<retreival_documents> common_documents = new List<retreival_documents> { };
        List<retreival_documents> retrieval = new List<retreival_documents> { };
        List<retreival_documents> retrieval_exactsearch = new List<retreival_documents> { };

        string[] common_words;
        public struct soundex_terms
        {
            public Decimal doc_ID;
            public Decimal freq;
        }
        List<soundex_terms> all_soundex_documents = new List<soundex_terms> { };

        static List<string> stop_words = new List<string>()
            {
                "a",
                "able",
                "about",
                "above",
                "according",
                "accordingly",
                "across",
                "actually",
                "after",
                "afterwards",
                "again",
                "against",
                "ain't",
                "all",
                "allow",
                "allows",
                "almost",
                "alone",
                "along",
                "already",
                "also",
                "although",
                "always",
                "am",
                "among",
                "amongst",
                "an",
                "and",
                "another",
                "any",
                "anybody",
                "anyhow",
                "anyone",
                "anything",
                "anyway",
                "anyways",
                "anywhere",
                "apart",
                "appear",
                "appreciate",
                "appropriate",
                "are",
                "aren't",
                "around",
                "as",
                "a's",
                "aside",
                "ask",
                "asking",
                "associated",
                "at",
                "available",
                "away",
                "awfully",
                "be",
                "became",
                "because",
                "become",
                "becomes",
                "becoming",
                "been",
                "before",
                "beforehand",
                "behind",
                "being",
                "believe",
                "below",
                "beside",
                "besides",
                "best",
                "better",
                "between",
                "beyond",
                "both",
                "brief",
                "but",
                "by",
                "came",
                "can",
                "cannot",
                "cant",
                "can't",
                "cause",
                "causes",
                "certain",
                "certainly",
                "changes",
                "clearly",
                "c'mon", "co", "com", "come", "comes",
                "concerning",
                "consequently",
                "consider",  "considering",
                "contain",
                "containing",  "contains",
                "corresponding",
                "could",
                "couldn't", "course",
                "c's",
                "currently", "definitely",
                "described",
                "despite",
                "did",
                "didn't",
                "different",
                "do",
                "does",
                "doesn't",
                "doing",
                "done",
                "don't",
                "down",
                "downwards",
                "during",
                "each",
                "edu",
                "eg",
                "eight",
                "either",
                "else",
                "elsewhere",
                "enough",
                "entirely",
                "especially",
                "et",
                "etc",
                "even",
                "ever",
                "every",
                "everybody",
                "everyone",
                "everything",
                "everywhere",
                "ex",
                "exactly",
                "example",
                "except",
                "far",
                "few",
                "fifth",
                "first",
                "five",
                "followed",
                "following",
                "follows",
                "for",
                "former",
                "formerly",
                "forth",
                "four",
                "from",
                "further",
                "furthermore",
                "get",
                "gets",
                "getting",
                "given",
                "gives",
                "go",
                "goes",
                "going",
                "gone",
                "got",
                "gotten",
                "greetings",
                "had",
                "hadn't",
                "happens",
                "hardly",
                "has",
                "hasn't",
                "have",
                "haven't",
                "having",
                "he",
                "he'd",
                "he'll",
                "hello",
                "help",
                "hence",
                "her",
                "here",
                "hereafter",
                "hereby",
                "herein",
                "here's",
                "hereupon",
                "hers",
                "herself",
                "he's",
                "hi",
                "him",
                "himself",
                "his",
                "hither",
                "hopefully",
                "how",
                "howbeit",
                "however",
                "how's",
                "i",
                "i'd",
                "ie",
                "if",
                "ignored",
                "i'll",
                "i'm",
                "immediate",
                "in",
                "inasmuch",
                "inc",
                "indeed",
                "indicate",
                "indicated",
                "indicates",
                "inner",
                "insofar",
                "instead",
                "into",
                "inward",
                "is",
                "isn't",
                "it",
                "it'd",
                "it'll",
                "its",
                "it's",
                "itself",
                "i've",
                "just",
                "keep",
                "keeps",
                "kept",
                "know",
                "known",
                "knows",
                "last",
                "lately",
                "later",
                "latter",
                "latterly",
                "least",
                "less",
                "lest",
                "let",
                "let's",
                "like",
                "liked",
                "likely",
                "little",
                "look",
                "looking",
                "looks",
                "ltd",
                "mainly",
                "many",
                "may",
                "maybe",
                "me",
                "mean",
                "meanwhile",
                "merely",
                "might",
                "more",
                "moreover",
                "most",
                "mostly",
                "much",
                "must",
                "mustn't",
                "my",
                "myself",
                "name",
                "namely",
                "nd",
                "near",
                "nearly",
                "necessary",
                "need",
                "needs",
                "neither",
                "never",
                "nevertheless",
                "new",
                "next",
                "nine",
                "no",
                "nobody",
                "non",
                "none",
                "noone",
                "nor",
                "normally",
                "not",
                "nothing",
                "novel",
                "now",
                "nowhere",
                "obviously",
                "of",
                "off",
                "often",
                "oh",
                "ok",
                "okay",
                "old",
                "on",
                "once",
                "one",
                "ones",
                "only",
                "onto",
                "or",
                "other",
                "others",
                "otherwise",
                "ought",
                "our",
                "ours",
                "ourselves",
                "out",
                "outside",
                "over",
                "overall",
                "own",
                "particular",
                "particularly",
                "per",
                "perhaps",
                "placed",
                "please",
                "plus",
                "possible",
                "presumably",
                "probably",
                "provides",
                "que",
                "quite",
                "qv",
                "rather",
                "rd",
                "re",
                "really",
                "reasonably",
                "regarding",
                "regardless",
                "regards",
                "relatively",
                "respectively",
                "right",
                "said",
                "same",
                "saw",
                "say",
                "saying",
                "says",
                "second",
                "secondly",
                "see",
                "seeing",
                "seem",
                "seemed",
                "seeming",
                "seems",
                "seen",
                "self",
                "selves",
                "sensible",
                "sent",
                "serious",
                "seriously",
                "seven",
                "several",
                "shall",
                "shan't",
                "she",
                "she'd",
                "she'll",
                "she's",
                "should",
                "shouldn't",
                "since",
                "six",
                "so",
                "some",
                "somebody",
                "somehow",
                "someone",
                "something",
                "sometime",
                "sometimes",
                "somewhat",
                "somewhere",
                "soon",
                "sorry",
                "specified",
                "specify",
                "specifying",
                "still",
                "sub",
                "such",
                "sup",
                "sure",
                "take",
                "taken",
                "tell",
                "tends",
                "th",
                "than",
                "thank",
                "thanks",
                "thanx",
                "that",
                "thats",
                "that's",
                "the",
                "their",
                "theirs",
                "them",
                "themselves",
                "then",
                "thence",
                "there",
                "thereafter",
                "thereby",
                "therefore",
                "therein",
                "theres",
                "there's",
                "thereupon",
                "these",
                "they",
                "they'd",
                "they'll",
                "they're",
                "they've",
                "think",
                "third",
                "this",
                "thorough",
                "thoroughly",
                "those",
                "though",
                "three",
                "through",
                "throughout",
                "thru",
                "thus",
                "to",
                "together",
                "too",
                "took",
                "toward",
                "towards",
                "tried",
                "tries",
                "truly",
                "try",
                "trying",
                "t's",
                "twice",
                "two",
                "un",
                "under",
                "unfortunately",
                "unless",
                "unlikely",
                "until",
                "unto",
                "up",
                "upon",
                "us",
                "use",
                "used",
                "useful",
                "uses",
                "using",    "usually", "value",  "various", "very",
                "via", "viz","vs", "want", "wants", "was", "wasn't","way", "we",
                "we'd", "welcome", "well", "we'll",
                "went", "were", "we're","weren't", "we've", "what",
                "whatever",  "what's","when","whence","whenever",
                "when's",  "where","whereafter",   "whereas", "whereby",  "wherein",  "where's",  "whereupon",
                "wherever", "whether", "which", "while", "whither", "who", "whoever",  "whole", "whom",
                "who's", "whose",  "why",  "why's","will", "willing", "wish", "with","within","without",
                "wonder","won't", "would", "wouldn't","yes","yet", "you","you'd", "you'll", "you're","yours", "your",
                "yourself","yourselves","you've","zero"
            };
        public Form1()
        {
            InitializeComponent();
        }
        public static int EditDistance(string a, string b)//Edit Distance
        {
            if (String.IsNullOrEmpty(a) || String.IsNullOrEmpty(b)) return 0;

            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                    distances[i, j] = Math.Min
                        (
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                        );
                }
            return distances[lengthA, lengthB];
        }
        public static List<string> SpellCorrection(string s)
        {
            Object RetrunedObjectFromDb;
            List<string> BiGramsOfEnteredWord = new List<string>();
            string KGram = s;
            KGram = "$" + KGram + "$";
            string ResultFromDb = "";
            List<string> AllTermsOfGrams = new List<string>();
            for (int j = 0; j < KGram.Length - 1; j++)
            {
                string CurGram = KGram[j].ToString() + KGram[j + 1].ToString();
                cmd = new SqlCommand("select term from Bi_Gram where k_gram  = @kgram ", con);
                cmd.Parameters.AddWithValue("@kgram", CurGram);
                RetrunedObjectFromDb = cmd.ExecuteScalar();
                ResultFromDb = System.Convert.ToString(RetrunedObjectFromDb);
                BiGramsOfEnteredWord.Add(CurGram);
                AllTermsOfGrams.Add(ResultFromDb);
            }

            List<string> UniqueAllTerm = new List<string>();
            for (int i = 0; i < AllTermsOfGrams.Count(); i++)
            {
                string[] SplittedTerms = AllTermsOfGrams[i].Split(',');
                for (int j = 0; j < SplittedTerms.Length; j++)
                {
                    if (!UniqueAllTerm.Contains(SplittedTerms[j]))
                    {
                        UniqueAllTerm.Add(SplittedTerms[j]);
                    }
                }
            }
            List<string> FinalTermsForEditDistance = new List<string>();
            for (int i = 0; i < UniqueAllTerm.Count(); i++)
            {
                string CurTerm = "$" + UniqueAllTerm[i] + "$";
                List<string> ListOfCurGrams = new List<string>();
                for (int j = 0; j < CurTerm.Length - 1; j++)
                {
                    string KGramTmp = CurTerm[j].ToString() + CurTerm[j + 1].ToString();
                    ListOfCurGrams.Add(KGramTmp);
                }
                var IntersectOfGrams = BiGramsOfEnteredWord.Intersect(ListOfCurGrams);
                int TmpCal = IntersectOfGrams.Count();

                double FinalResult = 2.0 * TmpCal / (float)(BiGramsOfEnteredWord.Count() + ListOfCurGrams.Count());

                if (FinalResult >= 0.45)
                {
                    FinalTermsForEditDistance.Add(UniqueAllTerm[i]);

                }
                ListOfCurGrams.Clear();
            }

            List<int> Indexes = new List<int>();
            List<int> EditDistanceRes = new List<int>();
            for (int i = 0; i < FinalTermsForEditDistance.Count(); i++)
            {
                Indexes.Add(i);
                EditDistanceRes.Add(EditDistance(s.ToLower(), FinalTermsForEditDistance[i]));
            }
            
            for (int i = 0; i < EditDistanceRes.Count() - 1; i++)
            {
                for (int j = i + 1; j < EditDistanceRes.Count(); j++)
                {
                    if (EditDistanceRes[i] > EditDistanceRes[j])
                    {
                        int Tmp = EditDistanceRes[i];
                        EditDistanceRes[i] = EditDistanceRes[j];
                        EditDistanceRes[j] = Tmp;

                        Tmp = Indexes[i];
                        Indexes[i] = Indexes[j];
                        Indexes[j] = Tmp;

                        string Tmp1 = FinalTermsForEditDistance[i];
                        FinalTermsForEditDistance[i] = FinalTermsForEditDistance[j];
                        FinalTermsForEditDistance[j] = Tmp1;
                    }
                }
            }

            List<string> FinalResultTerms = new List<string>();
            for (int i = 0; i < Indexes.Count; i++)
            {
                FinalResultTerms.Add(FinalTermsForEditDistance[Indexes[i]]);
            }
            return FinalResultTerms;

        }
        public void Display_soundex()
        {
            string ret = @"select Link from EnglishPages where Id=@id;";
            for (int i = 0; i < all_soundex_documents.Count; i++)
            {
                cmd = new SqlCommand(ret, con);
                cmd.Parameters.AddWithValue("@id", all_soundex_documents[i].doc_ID);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    textBox2.Text += " URL:  " + reader[0].ToString() + "   Frequancy:  " + all_soundex_documents[i].freq + "\r\n";
                }
                reader.Close();
            }
        }
        public void Display_Output()
        {
            textBox2.Text = "Common Resultes......";
            string ret = @"select Link from EnglishPages where Id=@id;";
            if(query_without_stopwords.Count == 1)
            {
                for (int i = 0; i < all_documents.Count; i++)
                {
                    cmd = new SqlCommand(ret, con);
                    cmd.Parameters.AddWithValue("@id", all_documents[i].document_ID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        textBox2.Text += "\r\n" + " URL:  " + reader[0].ToString() + "   frequancy:  " + all_documents[i].frequency;
                    }
                    reader.Close();
                }
            }
            else
            {
                for (int i = 0; i < retrieval.Count; i++)
                {
                    cmd = new SqlCommand(ret, con);
                    cmd.Parameters.AddWithValue("@id", retrieval[i].docID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        textBox2.Text += "\r\n" + " URL:  " + reader[0].ToString() + "   Min Distance:  " + retrieval[i].score;
                    }
                    reader.Close();
                }
                textBox2.Text = textBox2.Text + "\r\n" + "Non Comman Results ...";
                for (int i = 0; i < common_documents.Count; i++)
                {
                    textBox2.Text += "\r\n" + " Document ID:  " + common_documents[i].docID + "   Min Distance:  " + common_documents[i].score;
                }
            }
        }
        public void Display_Output_in_exactsearch()
        {
            textBox2.Text = "Common Resultes......";
            string ret = @"select Link from EnglishPages where Id=@id;";
            for (int i = 0; i < retrieval_exactsearch.Count; i++)
            {
                cmd = new SqlCommand(ret, con);
                cmd.Parameters.AddWithValue("@id", retrieval_exactsearch[i].docID);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    textBox2.Text += "\r\n" + " URL:  " + reader[0].ToString() + "   Frequancy:  " + retrieval_exactsearch[i].freq;
                }
                reader.Close();
            }
            textBox2.Text = textBox2.Text + "\r\n" + "Non Comman Results ...";
            for (int i = 0; i < common_documents.Count; i++)
            {
                textBox2.Text += "\r\n" + " Document ID:  " + common_documents[i].docID + "   Min Distance:  " + common_documents[i].score;
            }
        }
        public void select_documents()
        {
            string qq = @"select DOCID,frequancy,positions from Inverted_INDEX where stemm=@w;";
            term = new word_and_docID();
            for(int word = 1; word <= query_without_stopwords.Count; word++)
            {
                cmd = new SqlCommand(qq, con);
                cmd.Parameters.AddWithValue("@w", query_without_stopwords[word - 1]);
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    term.word_number = word;
                    term.document_ID = (Decimal)reader[0];
                    term.frequency = (Decimal)reader[1];
                    term.positions = reader[2].ToString();
                    all_documents.Add(term);
                }
                reader.Close();
                MessageBox.Show(query_without_stopwords[word - 1].ToString());
            }
        }
        public void select_soundex_documents()
        {
            soundex_terms ss = new soundex_terms();
            string qq = @"select DOCID,frequancy from Inverted_INDEX where stemm=@w;";
            for (int word = 0; word < common_words.Length; word++)
            {
                cmd = new SqlCommand(qq, con);
                cmd.Parameters.AddWithValue("@w", common_words[word]);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ss.doc_ID = (Decimal)reader[0];
                    ss.freq = (Decimal)reader[1];
                    all_soundex_documents.Add(ss);
                }
                reader.Close();
            }
            all_soundex_documents = all_soundex_documents.OrderByDescending(x => x.freq).ToList();
        }
        public void find_common()
        {
            term = new word_and_docID();
            word_and_docID term2 = new word_and_docID();
            retreival_documents retreival_struct =new retreival_documents();
            retreival_documents common =new retreival_documents();
            string[] poss1, poss2;
            int score1;
            int score_sum;
            if(query_without_stopwords.Count == 1)
            {
                all_documents = all_documents.OrderByDescending(x => x.frequency).ToList();
                MessageBox.Show(all_documents.Count.ToString());
            }
            else
            {
                for (int i = 0; i < all_documents.Count - 1; i++)
                {
                    score_sum = 0;
                    term = all_documents[i];
                    for (int word_pos = 1; word_pos < query_without_stopwords.Count; word_pos++)
                    {
                        if (term.word_number == word_pos)
                        {
                            score1 = 1000000;
                            for (int j = 0; j < all_documents.Count; j++)
                            {
                                term2 = all_documents[j];
                                if ((term2.word_number == word_pos + 1) && (term.document_ID == term2.document_ID))
                                {
                                    poss1 = term.positions.Split(',');
                                    poss2 = term2.positions.Split(',');
                                    for (int t1 = 0; t1 < poss2.Length; t1++)
                                    {
                                        for (int t2 = 0; t2 < poss1.Length; t2++)
                                        {
                                            int sc = (Convert.ToInt32(poss2[t1]) - Convert.ToInt32(poss1[t2]));
                                            if ((sc > 0) && (sc < score1))
                                            {
                                                score1 = sc;
                                            }
                                            else
                                            {
                                                if (score1 != 1000000)
                                                {
                                                    common.score = score1;
                                                    common.docID = term.document_ID;
                                                    if ((common_documents.Count < 20) && (!common_documents.Contains(common)))
                                                        common_documents.Add(common);
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            if (score1 != 1000000)
                                score_sum += score1;
                        }
                    }
                    if (score_sum != 0)
                    {
                        retreival_struct.score = score_sum;
                        retreival_struct.docID = term.document_ID;
                        retreival_struct.freq = term.frequency;
                        if (!retrieval.Contains(retreival_struct))
                            retrieval.Add(retreival_struct);
                    }
                }
                retrieval = retrieval.OrderBy(x => x.score).ToList();
                MessageBox.Show(retrieval.Count.ToString());
            }
        }
        public void find_common_in_exact_search()
        {
            term = new word_and_docID();
            word_and_docID term2 = new word_and_docID();
            retreival_documents retreival_struct = new retreival_documents();
            retreival_documents common = new retreival_documents();
            string[] poss1, poss2;
            int score1;
            int score_sum;
            for (int i = 0; i < all_documents.Count - 1; i++)
            {
                score_sum = 1;
                term = all_documents[i];
                for (int word_pos = 1; word_pos < query_without_stopwords.Count; word_pos++)
                {
                    if (term.word_number == word_pos)
                    {
                        score1 = 1000000;
                        for (int j = 0; j < all_documents.Count; j++)
                        {
                            term2 = all_documents[j];
                            if ((term2.word_number == word_pos + 1) && (term.document_ID == term2.document_ID))
                            {
                                poss1 = term.positions.Split(',');
                                poss2 = term2.positions.Split(',');
                                for (int t1 = 0; t1 < poss2.Length; t1++)
                                {
                                    for (int t2 = 0; t2 < poss1.Length; t2++)
                                    {
                                        int sc = (Convert.ToInt32(poss2[t1]) - Convert.ToInt32(poss1[t2]));
                                        if ((sc > 0) && (sc < score1))
                                        {
                                            score1 = sc;
                                        }
                                        else
                                        {
                                            if (score1 != 1000000)
                                            {
                                                common.score = score1;
                                                common.docID = term.document_ID;
                                                if ((common_documents.Count < 20) && (!common_documents.Contains(common)))
                                                   common_documents.Add(common);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        if (score1 != 1)
                            score_sum = 0;
                    }
                }
                if (score_sum == 1)
                {
                    retreival_struct.score = score_sum;
                    retreival_struct.docID = term.document_ID;
                    retreival_struct.freq = term.frequency;
                    if (!retrieval_exactsearch.Contains(retreival_struct))
                        retrieval_exactsearch.Add(retreival_struct);
                }
            }
            retrieval_exactsearch = retrieval_exactsearch.OrderByDescending(x => x.freq).ToList();
            MessageBox.Show(retrieval_exactsearch.Count.ToString());
        }
        public void multi_words()
        {
            //all_documents list have all doc that have word of query
            select_documents();
            find_common();
        }
        public void exact_match()
        {
            //all_documents list have all doc that have word of query
            select_documents();
            find_common_in_exact_search();
            flag = true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            listView1.Clear();
            listView2.Clear();
            con = new SqlConnection("Data Source=AHMED;Initial Catalog=SearchEngineSystem;Integrated Security=True");
            con.Open();
            string query = textBox1.Text.ToString().ToLower();
            if (query[0] == '"')
                flag = false;
            char[] separators = { ',', ' ', '(', ')', '/', '{', '}', '"', '.', '#', ':', ';', '?', '^', '!', '|', '<', '>' };
            string[] split = query.Split(separators);
            for(int i=0;i<split.Length;i++)
            {
                if (!stop_words.Contains(split[i]))
                    query_without_stopwords.Add(stemmer.StemWord(split[i]));
            }
            if (checkBox1.Checked && checkBox2.Checked)
                MessageBox.Show(" must select only one checkbox ");

            else if (checkBox1.Checked) // soundex
            {
                if (query_without_stopwords.Count != 1)
                    MessageBox.Show("you must enter only one word ..");
                else
                {
                    string sound = Soundex.get_soundex(query_without_stopwords[0].ToString());
                    MessageBox.Show(sound);
                    string select = @"select term from Soundex where soundex = @s;";
                    cmd = new SqlCommand(select, con);
                    cmd.Parameters.AddWithValue("@s", sound);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    common_words = reader[0].ToString().Split(',');
                    for (int i = 0; i < common_words.Length; i++)
                    {
                        listView2.Items.Add(common_words[i]);
                    }
                    reader.Close();
                    select_soundex_documents();
                    Display_soundex();
                }
            }

            else if (checkBox2.Checked) // spell connection
            {
                //ListOfWords.Items.Clear();
                //string[] EnteredWords = SearchQuery.Text.Split(' ');
                List<List<string>> NestedList = new List<List<string>>();
                for (int i = 0; i < query_without_stopwords.Count; i++)
                {
                    List<string> TList = SpellCorrection(query_without_stopwords[i]);
                    NestedList.Add(TList);

                }
               
                for (int j = 0; j < NestedList.Count(); j++)
                {
                    for (int i = 0; i < NestedList[j].Count; i++)
                    {
                        listView1.Items.Add(NestedList[j][i]);
                    }
                }
            }

            else
            {
                if (flag == true)
                {
                    multi_words();
                    Display_Output();
                }
                else
                {
                    MessageBox.Show("exact");
                    exact_match();
                    Display_Output_in_exactsearch();
                }
            }

            all_documents.Clear();
            retrieval.Clear();
            common_documents.Clear();
            retrieval_exactsearch.Clear();
            query_without_stopwords.Clear();
            MessageBox.Show(" Complete Successfully ");
            con.Close();
        }

      
    }
}
