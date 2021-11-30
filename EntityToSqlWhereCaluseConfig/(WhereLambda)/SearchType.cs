namespace EntityToSqlWhereCaluseConfig
{
    /// <summary>
    /// 
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
        /// ���ڵ�����
        /// ������ʼֵ,����������ֵ
        /// [xxxStart, xxxEnd)
        /// </summary>
        datetimeRange,

        /// <summary>
        /// ��ֵ�ķ�Χ
        /// ������ʼֵ�����ֵ
        /// [xxxLeft, xxxRight]
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
