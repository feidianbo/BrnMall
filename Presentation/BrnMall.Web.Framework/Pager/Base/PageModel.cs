using System;

namespace BrnMall.Web.Framework
{
    /// <summary>
    /// ��ҳģ��
    /// </summary>
    public class PageModel
    {
        private int _pageindex;//��ǰҳ����
        private int _pagenumber;//��ǰҳ��
        private int _prepagenumber;//��һҳ��
        private int _nextpagenumber;//��һҳ��
        private int _pagesize;//ÿҳ��
        private int _totalcount;//������
        private int _totalpages;//��ҳ��
        private bool _hasprepage;//�Ƿ�����һҳ
        private bool _hasnextpage;//�Ƿ�����һҳ
        private bool _isfirstpage;//�Ƿ��ǵ�һҳ
        private bool _islastpage;//�Ƿ������һҳ

        public PageModel(int pageSize, int pageNumber, int totalCount)
        {
            if (pageSize > 0)
                _pagesize = pageSize;
            else
                _pagesize = 1;

            if (pageNumber > 0)
                _pagenumber = pageNumber;
            else
                _pagenumber = 1;

            if (totalCount > 0)
                _totalcount = totalCount;
            else
                _totalcount = 0;

            _pageindex = _pagenumber - 1;

            _totalpages = _totalcount / _pagesize;
            if (_totalcount % _pagesize > 0)
                _totalpages++;

            _hasprepage = _pagenumber > 1;
            _hasnextpage = _pagenumber < _totalpages;

            _isfirstpage = _pagenumber == 1;
            _islastpage = _pagenumber == _totalpages;

            _prepagenumber = _pagenumber < 2 ? 1 : _pagenumber - 1;
            _nextpagenumber = _pagenumber < _totalpages ? _pagenumber + 1 : _totalpages;
        }

        /// <summary>
        /// ��ǰҳ����
        /// </summary>
        public int PageIndex
        {
            get { return _pageindex; }
            set { _pageindex = value; }
        }
        /// <summary>
        /// ��ǰҳ��
        /// </summary>
        public int PageNumber
        {
            get { return _pagenumber; }
            set { _pagenumber = value; }
        }
        /// <summary>
        /// ��һҳ��
        /// </summary>
        public int PrePageNumber
        {
            get { return _prepagenumber; }
            set { _prepagenumber = value; }
        }
        /// <summary>
        /// ��һҳ��
        /// </summary>
        public int NextPageNumber
        {
            get { return _nextpagenumber; }
            set { _nextpagenumber = value; }
        }
        /// <summary>
        /// ÿҳ��
        /// </summary>
        public int PageSize
        {
            get { return _pagesize; }
            set { _pagesize = value; }
        }
        /// <summary>
        /// ������
        /// </summary>
        public int TotalCount
        {
            get { return _totalcount; }
            set { _totalcount = value; }
        }
        /// <summary>
        /// ��ҳ��
        /// </summary>
        public int TotalPages
        {
            get { return _totalpages; }
            set { _totalpages = value; }
        }
        /// <summary>
        /// �Ƿ�����һҳ
        /// </summary>
        public bool HasPrePage
        {
            get { return _hasprepage; }
            set { _hasprepage = value; }
        }
        /// <summary>
        /// �Ƿ�����һҳ
        /// </summary>
        public bool HasNextPage
        {
            get { return _hasnextpage; }
            set { _hasnextpage = value; }
        }
        /// <summary>
        /// �Ƿ��ǵ�һҳ
        /// </summary>
        public bool IsFirstPage
        {
            get { return _isfirstpage; }
            set { _isfirstpage = value; }
        }
        /// <summary>
        /// �Ƿ������һҳ
        /// </summary>
        public bool IsLastPage
        {
            get { return _islastpage; }
            set { _islastpage = value; }
        }
    }
}