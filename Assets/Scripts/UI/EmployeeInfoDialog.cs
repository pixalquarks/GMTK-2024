using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeInfoDialog : MonoBehaviour
{
    public static EmployeeInfoDialog main;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI nameLabel, typeLabel;
    [SerializeField] private Image typeIcon;

    [Header("Skillset")]
    [SerializeField] private LineRenderer line;
    [SerializeField] private float pentagonRadius = 31.92f;
    [SerializeField] private Transform listParent;

    [Header("Stats")]
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI expLabel;
    [SerializeField] private TextMeshProUGUI spLabel, loadLabel, speedBonusLabel, revenueBonusLabel;

    [Header("Traits")]
    [SerializeField] private TextMeshProUGUI salaryLabel;

    private TextMeshProUGUI[] skillsetLabels = new TextMeshProUGUI[5];
    private Button[] skillsetButtons = new Button[5];
    public Employee employee;

    private bool init = false;

    private void Update()
    {
        if(employee is not null)
        {
            transform.position = employee.transform.position;
        }
    }

    private void Init()
    {
        for (int i = 0; i < 5; i++)
        {
            int ii = i;
            Transform c = listParent.GetChild(i);
            skillsetLabels[i] = c.GetChild(1).GetComponent<TextMeshProUGUI>();
            Button b = c.GetChild(2).GetComponent<Button>();
            skillsetButtons[i] = b;
            b.onClick.AddListener(() => UpgradeSkill(ii));

            if(i == 4) b.gameObject.SetActive(false);
        }
        init = true;
    }

    public void Rebuild()
    {
        if (!init) Init();
        nameLabel.text = employee.displayName;
        typeLabel.text = employee.type.name;
        typeLabel.color = employee.type.color;
        typeIcon.sprite = employee.type.icon;
        typeIcon.color = employee.type.color;

        for(int i = 0; i < 5; i++)
        {
            string s = "";
            float v = employee.baseSkillset.GetValue(i);
            float extra = employee.calculatedSkillBonus.GetValue(i);
            if(extra > 0.05f)
            {
                s = $" <color=#00aa00>+{extra:F2}";
            }
            else if(extra < -0.05f)
            {
                s = $" <color=#aa0000>-{-extra:F2}";
            }
            skillsetLabels[i].text = $"{v:F2}{s}";

            if(i != 4) skillsetButtons[i].interactable = employee.CanSpendSkillPoint(i);
            SetPoint(i, v);
        }

        SetBar(expBar, employee.ExpFraction);
        expLabel.text = $"EXP: {employee.Exp}/{employee.GetRequiredExp()}";

        spLabel.text = $"SP: {employee.SkillPoints}";
        loadLabel.text = $"Load: {employee.GetLoad():F2}";
        revenueBonusLabel.text = $"Revenue+: {employee.GetBonusRevenue()*100:F2}%";
        speedBonusLabel.text = $"Speed+: {employee.GetBonusSpeed()*100:F2}%";

        salaryLabel.text = $"${employee.GetSalary():N0}/Q";
    }

    private void SetBar(Image i, float f)
    {
        i.rectTransform.anchorMin = Vector2.zero;
        i.rectTransform.anchorMax = new Vector2(f, 1);
    }

    private void UpgradeSkill(int ii)
    {
        if(employee.CanSpendSkillPoint(ii))
        {
            employee.SpendSkillPoint(ii);
        }
    }

    private void SetPoint(int id, float amount)
    {
        amount = Mathf.Clamp(amount, 0f, 5f);
        float theta = Mathf.PI * 0.4f * id + Mathf.PI * 0.5f;
        line.SetPosition(id, new Vector3(Mathf.Cos(theta) * pentagonRadius * amount, Mathf.Sin(theta) * pentagonRadius * amount));
    }
}