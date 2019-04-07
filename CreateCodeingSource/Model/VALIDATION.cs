using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateCodeingSource.Model
{
    class VALIDATION
    {
        [DisplayName("項目")]
        public string itemName { get; set; }
        [DisplayName("タイプ")]
        public string type { get; set; }
        [DisplayName("Nullable")]
        public Boolean isNull { get; set; } = true;
        [DisplayName("文字列長")]
        public long? length { get; set; }
        [DisplayName("最小数")]
        public long? minAmount { get; set; }
        [DisplayName("最大文字数")]
        public long? maxAmount { get; set; }
        [DisplayName("半角数")]
        public Boolean hankakuSuu { get; set; }
        [DisplayName("半角英数")]
        public Boolean hankakuEisuu { get; set; }
        [DisplayName("半角英数記号")]
        public Boolean hankakuEisuuKigou { get; set; }
        [DisplayName("半角帯小数")]
        public Boolean hankakuObi { get; set; }
        [DisplayName("その他正規表現")]
        public string anotherRegex { get; set; }
    }
}
