//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebTest2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class test_case_table
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public test_case_table()
        {
            this.test_scenario_table = new HashSet<test_scenario_table>();
        }
    
        public int test_case_id { get; set; }
        public string test_case_data { get; set; }
        public string expected_result { get; set; }
        public string test_round { get; set; }
        public string result { get; set; }
        public System.DateTime date { get; set; }
        public string test_by { get; set; }
        public string path_test_case { get; set; }
        public string path_expected_result { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<test_scenario_table> test_scenario_table { get; set; }
    }
}
