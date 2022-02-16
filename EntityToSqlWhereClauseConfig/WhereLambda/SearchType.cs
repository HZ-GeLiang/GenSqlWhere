// ReSharper disable once CheckNamespace
namespace EntityToSqlWhereClauseConfig
{
    public class SearchTypeAttribute : System.Attribute
    {
        public SearchType SearchType { get; private set; }

        public SearchTypeAttribute(SearchType searchType)
        {
            this.SearchType = searchType;
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// δ����
        /// </summary>
        none = 0,

        /// <summary>
        /// like %%
        /// </summary>
        like,

        /// <summary>
        /// ֻ�ᷭ��� Equal(=��) �����ݿ��ֵ����,��ʱ����Equal��ѯʱ, ��ֻ�������
        /// </summary>
        eq,

        /// <summary>
        /// ʵ�ʷ���� in ���� Equal , ���� split() ��ĸ�������
        /// </summary>
        @in,

        /// <summary>
        /// ���ڵ�����(ֻ��������),����ʱ����ڵ�ǰ����+1,Ȼ��ʹ�� С�ڷ���, ��: >=@xxx And < @xxx1
        /// 1:�����䷶Χ [xxxStart, xxxEnd]=> ������ʼֵ,��������ֵ
        /// 2:û�����䷶Χ  xxx =>(ȡ��ǰʱ�侫��: �� ��ǰ����, ��ǰ��һСʱ, ��ǰ��һ����, ��ǰ��һ��)
        /// </summary>
        datetimeRange,

        /// <summary>
        /// ��ֵ�ķ�Χ(ֻ��������)
        /// 1:�����䷶Χ [xxxLeft, xxxRight]=> ������ʼֵ,��������ֵ
        /// 2:û�����䷶Χ  xxx => ��������Ĺ���, ��: >=@xxx And <= @xxx1
        /// </summary>
        numberRange,

        /// <summary>
        ///  greater than
        /// </summary>
        gt,


        /// <summary>
        /// ���ڻ����(LE)
        /// </summary>
        ge,

        /// <summary>
        ///  less Than
        /// </summary>
        lt,

        /// <summary>
        /// С�ڻ����(LE)
        /// </summary>
        le,

        /// <summary>
        /// not equal
        /// </summary>
        neq,

        /// <summary>
        /// ����� like '%xxx'
        /// </summary>
        likeLeft,

        /// <summary>
        /// ����� like 'xxx%'
        /// </summary>
        likeRight,

    }
}