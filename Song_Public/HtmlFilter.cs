using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Song_Public
{

    /// <summary>
    /// HTML字符的筛选
    /// </summary>
    public class HtmlFilter
    {
        /** regex flag union representing /si modifiers in php **/
        protected static readonly RegexOptions REGEX_FLAGS_SI = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled;

        /** regex flag union representing /si modifiers in php **/
        private static string P_COMMENTS = "<!--(.*?)-->";//, Pattern.DOTALL);
        private static Regex P_COMMENT = new Regex("^!--(.*)--$", REGEX_FLAGS_SI);
        private static string P_TAGS = "<(.*?)>";//, Pattern.DOTALL);
        private static Regex P_END_TAG = new Regex("^/([a-z0-9]+)", REGEX_FLAGS_SI);
        private static Regex P_START_TAG = new Regex("^([a-z0-9]+)(.*?)(/?)$", REGEX_FLAGS_SI);
        private static Regex P_QUOTED_ATTRIBUTES = new Regex("([a-z0-9]+)=([\"'])(.*?)\\2", REGEX_FLAGS_SI);
        private static Regex P_UNQUOTED_ATTRIBUTES = new Regex("([a-z0-9]+)(=)([^\"\\s']+)", REGEX_FLAGS_SI);
        private static Regex P_PROTOCOL = new Regex("^([^:]+):", REGEX_FLAGS_SI);
        private static Regex P_ENTITY = new Regex("&#(\\d+);?");
        private static Regex P_ENTITY_UNICODE = new Regex("&#x([0-9a-f]+);?");
        private static Regex P_ENCODE = new Regex("%([0-9a-f]{2});?");
        private static Regex P_VALID_ENTITIES = new Regex("&([^&;]*)(?=(;|&|$))");
        private static Regex P_VALID_QUOTES = new Regex("(>|^)([^<]+?)(<|$)", RegexOptions.Singleline | RegexOptions.Compiled);
        private static string P_END_ARROW = "^>";//);
        private static string P_BODY_TO_END = "<([^>]*?)(?=<|$)";//);
        private static string P_XML_CONTENT = "(^|>)([^<]*?)(?=>)";//);
        private static string P_STRAY_LEFT_ARROW = "<([^>]*?)(?=<|$)";//);
        private static string P_STRAY_RIGHT_ARROW = "(^|>)([^<]*?)(?=>)";//);
        private static string P_AMP = "&";//);
        private static string P_QUOTE = "\"";//);
        private static string P_LEFT_ARROW = "<";//);
        private static string P_RIGHT_ARROW = ">";//);
        private static string P_BOTH_ARROWS = "<>";//);

        // @xxx could grow large... maybe use sesat's ReferenceMap
        private static Dictionary<String, string> P_REMOVE_PAIR_BLANKS = new Dictionary<String, string>();
        private static Dictionary<String, string> P_REMOVE_SELF_BLANKS = new Dictionary<String, string>();
        /** 
         * flag determining whether to try to make tags when presented with "unbalanced"
         * angle brackets (e.g. "<b text </b>" becomes "<b> text </b>").  If set to false,
         * unbalanced angle brackets will be html escaped.
         */
        protected static bool alwaysMakeTags = true;

        /**
         * flag determing whether comments are allowed in input String.
         */
        protected static bool stripComment = true;


        /** set of disallowed html elements **/
        private String[] vDisallowed;
        /** set of allowed html elements, along with allowed attributes for each element **/
        protected Dictionary<String, List<String>> vAllowed;

        /** counts of open tags for each (allowable) html element **/
        protected Dictionary<String, int> vTagCounts;

        /** html elements which must always be self-closing (e.g. "<img />") **/
        protected String[] vSelfClosingTags;

        /** html elements which must always have separate opening and closing tags (e.g. "<b></b>") **/
        protected String[] vNeedClosingTags;

        /** attributes which should be checked for valid protocols **/
        protected String[] vProtocolAtts;

        /** allowed protocols **/
        protected String[] vAllowedProtocols;

        /** tags which should be removed if they contain no content (e.g. "<b></b>" or "<b />") **/
        protected String[] vRemoveBlanks;

        /** entities allowed within html markup **/
        protected String[] vAllowedEntities;



        protected bool vDebug;

        public HtmlFilter()
            : this(false)
        {
        }

        public HtmlFilter(bool debug)
        {
            vDebug = debug;

            vAllowed = new Dictionary<String, List<String>>();
            vTagCounts = new Dictionary<String, int>();

            List<String> a_atts = new List<String>();
            a_atts.Add("href");
            a_atts.Add("target");
            vAllowed.Add("a", a_atts);

            List<String> img_atts = new List<String>();
            img_atts.Add("src");
            img_atts.Add("width");
            img_atts.Add("height");
            img_atts.Add("alt");
            vAllowed.Add("img", img_atts);

            List<String> no_atts = new List<String>();
            vAllowed.Add("b", no_atts);
            vAllowed.Add("strong", no_atts);
            vAllowed.Add("i", no_atts);
            vAllowed.Add("em", no_atts);

            vSelfClosingTags = new String[] { "img" };
            vNeedClosingTags = new String[] { "a", "b", "strong", "i", "em" };
            vDisallowed = new String[] { };
            vAllowedProtocols = new String[] { "http", "mailto" }; // no ftp.
            vProtocolAtts = new String[] { "src", "href" };
            vRemoveBlanks = new String[] { "a", "b", "strong", "i", "em" };
            vAllowedEntities = new String[] { "amp", "gt", "lt", "quot" };
            stripComment = true;
            alwaysMakeTags = true;
        }


        protected void reset()
        {
            vTagCounts = new Dictionary<String, int>();
        }

        protected void debug(String msg)
        {
            if (vDebug)
                System.Diagnostics.Debug.WriteLine(msg);
        }

        //---------------------------------------------------------------
        // my versions of some PHP library functions

        public static String chr(int dec)
        {
            return "" + ((char)dec);
        }

        /// <summary>
        /// 转换成实体字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String HtmlSpecialChars(String str)
        {
            str = str.Replace(P_QUOTE, "&quot;");
            str = str.Replace(P_LEFT_ARROW, "&lt;");
            str = str.Replace(P_RIGHT_ARROW, "&gt;");
            str = str.Replace("\n", "<br>");
            return str;
        }

        //---------------------------------------------------------------

        /**
         * given a user submitted input String, filter out any invalid or restricted
         * html.
         * 
         * @param input text (i.e. submitted by a user) than may contain html
         * @return "clean" version of input, with only valid, whitelisted html elements allowed
         */
        public String filter(String input)
        {
            reset();
            String s = input;

            debug("************************************************");
            debug("              INPUT: " + input);

            s = escapeComments(s);
            debug("     escapeComments: " + s);

            s = balanceHTML(s);
            debug("        balanceHTML: " + s);

            s = checkTags(s);
            debug("          checkTags: " + s);

            s = processRemoveBlanks(s);
            debug("processRemoveBlanks: " + s);

            s = validateEntities(s);
            debug("    validateEntites: " + s);

            debug("************************************************\n\n");
            return s;
        }

        protected String escapeComments(String s)
        {
            return Regex.Replace(s, P_COMMENTS, new MatchEvaluator(ConverMatchComments), RegexOptions.Singleline);
        }

        protected String regexReplace(String regex_pattern, String replacement, String s)
        {
            return Regex.Replace(s, regex_pattern, replacement);
        }

        protected String balanceHTML(String s)
        {
            if (alwaysMakeTags)
            {
                //
                // try and form html
                //
                s = regexReplace(P_END_ARROW, "", s);
                s = regexReplace(P_BODY_TO_END, "<$1>", s);
                s = regexReplace(P_XML_CONTENT, "$1<$2", s);

            }
            else
            {
                //
                // escape stray brackets
                //
                s = regexReplace(P_STRAY_LEFT_ARROW, "&lt;$1", s);
                s = regexReplace(P_STRAY_RIGHT_ARROW, "$1$2&gt;<", s);

                //
                // the last regexp causes '<>' entities to appear
                // (we need to do a lookahead assertion so that the last bracket can
                // be used in the next pass of the regexp)
                //
                s = s.Replace(P_BOTH_ARROWS, "");
            }
            return s;
        }

        protected String checkTags(String s)
        {
            s = Regex.Replace(s, P_TAGS, new MatchEvaluator(ConverMatchTags), RegexOptions.Singleline);

            // these get tallied in processTag
            // (remember to reset before subsequent calls to filter method)
            foreach (String key in vTagCounts.Keys)
            {
                for (int ii = 0; ii < vTagCounts[key]; ii++)
                {
                    s += "</" + key + ">";
                }
            }

            return s;
        }

        protected String processRemoveBlanks(String s)
        {
            foreach (String tag in vRemoveBlanks)
            {
                s = regexReplace("<" + tag + "(\\s[^>]*)?></" + tag + ">", "", s);
                s = regexReplace("<" + tag + "(\\s[^>]*)?/>", "", s);
            }
            return s;
        }

        private String processTag(String s)
        {
            // ending tags
            Match m = P_END_TAG.Match(s);
            if (m.Success)
            {
                string name = m.Groups[1].Value.ToLower();
                if (allowed(name))
                {
                    if (!inArray(name, vSelfClosingTags))
                    {
                        if (vTagCounts.ContainsKey(name))
                        {
                            vTagCounts[name] = vTagCounts[name] - 1;
                            return "</" + name + ">";
                        }
                    }
                }
            }


            // starting tags
            m = P_START_TAG.Match(s);
            if (m.Success)
            {
                String name = m.Groups[1].Value.ToLower();
                String body = m.Groups[2].Value;
                String ending = m.Groups[3].Value;

                //debug( "in a starting tag, name='" + name + "'; body='" + body + "'; ending='" + ending + "'" );
                if (allowed(name))
                {
                    String params1 = "";

                    MatchCollection m2 = P_QUOTED_ATTRIBUTES.Matches(body);
                    MatchCollection m3 = P_UNQUOTED_ATTRIBUTES.Matches(body);
                    List<String> paramNames = new List<String>();
                    List<String> paramValues = new List<String>();
                    foreach (Match match in m2)
                    {
                        paramNames.Add(match.Groups[1].Value); //([a-z0-9]+)
                        paramValues.Add(match.Groups[3].Value); //(.*?)
                    }
                    foreach (Match match in m3)
                    {
                        paramNames.Add(match.Groups[1].Value); //([a-z0-9]+)
                        paramValues.Add(match.Groups[3].Value); //([^\"\\s']+)
                    }

                    String paramName, paramValue;
                    for (int ii = 0; ii < paramNames.Count; ii++)
                    {
                        paramName = paramNames[ii].ToLower();
                        paramValue = paramValues[ii];

                        if (allowedAttribute(name, paramName))
                        {
                            if (inArray(paramName, vProtocolAtts))
                            {
                                paramValue = processParamProtocol(paramValue);
                            }
                            params1 += " " + paramName + "=\"" + paramValue + "\"";
                        }
                    }

                    if (inArray(name, vSelfClosingTags))
                    {
                        ending = " /";
                    }

                    if (inArray(name, vNeedClosingTags))
                    {
                        ending = "";
                    }

                    if (ending == null || ending.Length < 1)
                    {
                        if (vTagCounts.ContainsKey(name))
                        {
                            vTagCounts[name] = vTagCounts[name] + 1;
                        }
                        else
                        {
                            vTagCounts.Add(name, 1);
                        }
                    }
                    else
                    {
                        ending = " /";
                    }
                    return "<" + name + params1 + ending + ">";
                }
                else
                {
                    return "";
                }
            }

            // comments
            m = P_COMMENT.Match(s);
            if (!stripComment && m.Success)
            {
                return "<" + m.Value + ">";
            }

            return "";
        }

        private String processParamProtocol(String s)
        {
            s = decodeEntities(s);
            Match m = P_PROTOCOL.Match(s);
            if (m.Success)
            {
                String protocol = m.Groups[1].Value;
                if (!inArray(protocol, vAllowedProtocols))
                {
                    // bad protocol, turn into local anchor link instead
                    s = "#" + s.Substring(protocol.Length + 1, s.Length - protocol.Length - 1);
                    if (s.StartsWith("#//"))
                    {
                        s = "#" + s.Substring(3, s.Length - 3);
                    }
                }
            }
            return s;
        }

        private String decodeEntities(String s)
        {

            s = P_ENTITY.Replace(s, new MatchEvaluator(ConverMatchEntity));

            s = P_ENTITY_UNICODE.Replace(s, new MatchEvaluator(ConverMatchEntityUnicode));

            s = P_ENCODE.Replace(s, new MatchEvaluator(ConverMatchEntityUnicode));

            s = validateEntities(s);
            return s;
        }

        private String validateEntities(String s)
        {
            s = P_VALID_ENTITIES.Replace(s, new MatchEvaluator(ConverMatchValidEntities));
            s = P_VALID_QUOTES.Replace(s, new MatchEvaluator(ConverMatchValidQuotes));
            return s;
        }

        private static bool inArray(String s, String[] array)
        {
            foreach (String item in array)
            {
                if (item != null && item.Equals(s))
                {
                    return true;
                }
            }
            return false;
        }

        private bool allowed(String name)
        {
            return (vAllowed.Count == 0 || vAllowed.ContainsKey(name)) && !inArray(name, vDisallowed);
        }

        private bool allowedAttribute(String name, String paramName)
        {
            return allowed(name) && (vAllowed.Count == 0 || vAllowed[name].Contains(paramName));
        }

        private String checkEntity(String preamble, String term)
        {

            return ";".Equals(term) && isValidEntity(preamble)
                    ? '&' + preamble
                    : "&amp;" + preamble;
        }
        private bool isValidEntity(String entity)
        {
            return inArray(entity, vAllowedEntities);
        }
        private static string ConverMatchComments(Match match)
        {
            string matchValue = "<!--" + HtmlSpecialChars(match.Groups[1].Value) + "-->";
            return matchValue;
        }

        private string ConverMatchTags(Match match)
        {
            string matchValue = processTag(match.Groups[1].Value);
            return matchValue;
        }

        private string ConverMatchEntity(Match match)
        {
            string v = match.Groups[1].Value;
            int decimal1 = int.Parse(v);
            return chr(decimal1);
        }

        private string ConverMatchEntityUnicode(Match match)
        {
            string v = match.Groups[1].Value;
            int decimal1 = Convert.ToInt32("0x" + v, 16);
            return chr(decimal1);
        }

        private string ConverMatchValidEntities(Match match)
        {
            String one = match.Groups[1].Value; //([^&;]*)
            String two = match.Groups[2].Value; //(?=(;|&|$))
            return checkEntity(one, two);
        }
        private string ConverMatchValidQuotes(Match match)
        {
            String one = match.Groups[1].Value; //(>|^)
            String two = match.Groups[2].Value; //([^<]+?)
            String three = match.Groups[3].Value;//(<|$)
            return one + regexReplace(P_QUOTE, "&quot;", two) + three;
        }

        public bool IsAlwaysMakeTags()
        {
            return alwaysMakeTags;
        }

        public bool IsStripComments()
        {
            return stripComment;
        }
    }
}
