using System;

namespace Server
{
	public class ResponseHandler<T>
	{
		private Action<T> m_okCallback;

		private Func<HttpC, T> m_parser;

		public ResponseHandler(Action<T> _okCallback, Func<HttpC, T> _parser)
		{
			m_okCallback = _okCallback;
			m_parser = _parser;
		}

		public void RequestOk(HttpC req)
		{
			T obj = m_parser(req);
			m_okCallback(obj);
		}
	}
}
