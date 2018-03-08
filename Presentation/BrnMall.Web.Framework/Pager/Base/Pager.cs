using System;

namespace BrnMall.Web.Framework
{
    /// <summary>
    /// ��ҳ����
    /// </summary>
    public abstract class Pager
    {
        protected readonly PageModel _pagemodel;//��ҳ����
        protected bool _showsummary = true;//�Ƿ���ʾ����
        protected bool _showitems = true;//�Ƿ���ʾҳ��
        protected int _itemcount = 7;//������
        protected bool _showfirst = true;//�Ƿ���ʾ��ҳ
        protected bool _showpre = true;//�Ƿ���ʾ��һҳ
        protected bool _shownext = true;//�Ƿ���ʾ��һҳ
        protected bool _showlast = true;//�Ƿ���ʾĩҳ
        protected bool _showpagesize = true;//�Ƿ���ʾÿҳ��
        protected bool _showgopage = true;//�Ƿ���ʾҳ�������

        public Pager(PageModel pageModel)
        {
            _pagemodel = pageModel;
        }

        /// <summary>
        /// �����Ƿ���ʾ����
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        public Pager ShowSummary(bool value)
        {
            _showsummary = value;
            return this;
        }
        /// <summary>
        /// �����Ƿ���ʾҳ��
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        public Pager ShowItems(bool value)
        {
            _showitems = value;
            return this;
        }
        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        public Pager ItemCount(int count)
        {
            _itemcount = count;
            return this;
        }
        /// <summary>
        /// �����Ƿ���ʾ��ҳ
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        public Pager ShowFirst(bool value)
        {
            _showfirst = value;
            return this;
        }
        /// <summary>
        /// �����Ƿ���ʾ��һҳ
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        public Pager ShowPre(bool value)
        {
            _showpre = value;
            return this;
        }
        /// <summary>
        /// �����Ƿ���ʾ��һҳ
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        public Pager ShowNext(bool value)
        {
            _shownext = value;
            return this;
        }
        /// <summary>
        /// �����Ƿ���ʾĩҳ
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        public Pager ShowLast(bool value)
        {
            _showlast = value;
            return this;
        }
        /// <summary>
        /// �����Ƿ���ʾÿҳ��
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        public Pager ShowPageSize(bool value)
        {
            _showpagesize = value;
            return this;
        }
        /// <summary>
        /// �����Ƿ���ʾҳ�������
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        public Pager ShowGoPage(bool value)
        {
            _showgopage = value;
            return this;
        }
        /// <summary>
        /// ��ÿ�ʼҳ��
        /// </summary>
        /// <returns></returns>
        protected int GetStartPageNumber()
        {
            int mid = _itemcount / 2;
            if ((_pagemodel.TotalPages < _itemcount) || ((_pagemodel.PageNumber - mid) < 1))
            {
                return 1;
            }
            if ((_pagemodel.PageNumber + mid) > _pagemodel.TotalPages)
            {
                return _pagemodel.TotalPages - _itemcount + 1;
            }
            return _pagemodel.PageNumber - mid;
        }
        /// <summary>
        /// ��ý���ҳ��
        /// </summary>
        /// <returns></returns>
        protected int GetEndPageNumber()
        {
            int mid = _itemcount / 2;
            if ((_itemcount % 2) == 0)
            {
                mid--;
            }
            if ((_pagemodel.TotalPages < _itemcount) || ((_pagemodel.PageNumber + mid) >= _pagemodel.TotalPages))
            {
                return _pagemodel.TotalPages;
            }
            if ((_pagemodel.PageNumber - (_itemcount / 2)) < 1)
            {
                return _itemcount;
            }
            return _pagemodel.PageNumber + mid;
        }
    }
}